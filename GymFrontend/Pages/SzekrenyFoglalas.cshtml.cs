using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace GymFrontend.Pages
{
    public class SzekrenyFoglalasModel : PageModel
    {
        public List<SzekrenyView> Szekrenyek { get; set; } = new();

        public async Task OnGetAsync()
        {
            using var client = new HttpClient();

            var token = HttpContext.Session.GetString("token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var res = await client.GetAsync("https://localhost:7270/api/SzekrenyFoglalasok");
            var json = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
            {
                for (int i = 1; i <= 50; i++)
                {
                    Szekrenyek.Add(new SzekrenyView
                    {
                        SzekrenyId = i,
                        SzekrenySzam = i,
                        Foglalt = false,
                        Enyem = false,
                        Zarva = false
                    });
                }
                return;
            }

            var foglalasok = JsonSerializer.Deserialize<List<Foglalas>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            var userId = GetUserIdFromToken(token);

            for (int i = 1; i <= 50; i++)
            {
                var foglalas = foglalasok.FirstOrDefault(f => f.SzekrenyId == i);

                Szekrenyek.Add(new SzekrenyView
                {
                    SzekrenyId = i,
                    SzekrenySzam = i,
                    Foglalt = foglalas != null,
                    Enyem = foglalas != null && foglalas.TagId == userId,
                    Zarva = foglalas != null && foglalas.Zarva
                });
            }
        }

        public async Task<IActionResult> OnPostFoglalAsync(int szekrenyId)
        {
            using var client = new HttpClient();
            var token = HttpContext.Session.GetString("token");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // 🔥 foglalás elküldése
            var dto = new
            {
                SzekrenyId = szekrenyId,
                Zarva = false,
                FoglalvaKezdete = DateTime.Now,
                FoglalvaVege = DateTime.Now.AddHours(2)
            };

            var content = new StringContent(
                JsonSerializer.Serialize(dto),
                Encoding.UTF8,
                "application/json"
            );

            var postRes = await client.PostAsync(
                "https://localhost:7270/api/SzekrenyFoglalasok",
                content
            );

            // 🔥 ha hiba → ne próbálj JSON-t olvasni!
            if (!postRes.IsSuccessStatusCode)
            {
                var error = await postRes.Content.ReadAsStringAsync();
                TempData["Hiba"] = "HIBA: " + error;
                return RedirectToPage();
            }

            TempData["Siker"] = "Sikeres foglalás!";
            return RedirectToPage();
        }

        // 🔥 ZÁR / NYIT
        public async Task<IActionResult> OnPostToggleAsync(int foglalasId, bool zarva)
        {
            using var client = new HttpClient();

            var token = HttpContext.Session.GetString("token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var dto = new
            {
                Zarva = !zarva
            };

            var content = new StringContent(
                JsonSerializer.Serialize(dto),
                Encoding.UTF8,
                "application/json"
            );

            await client.PutAsync($"https://localhost:7270/api/SzekrenyFoglalasok/{foglalasId}", content);

            return RedirectToPage();
        }

        private int GetUserIdFromToken(string token)
        {
            var parts = token.Split('.');
            var payload = parts[1];

            var jsonBytes = Convert.FromBase64String(
                payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=')
                .Replace('-', '+')
                .Replace('_', '/')
            );

            var json = Encoding.UTF8.GetString(jsonBytes);
            var data = JsonSerializer.Deserialize<JsonElement>(json);

            if (data.TryGetProperty("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", out var id))
                return int.Parse(id.GetString());

            throw new Exception("Nincs userId a tokenben!");
        }
    }

    public class SzekrenyView
    {
        public int SzekrenyId { get; set; }
        public int SzekrenySzam { get; set; }
        public bool Foglalt { get; set; }
        public bool Enyem { get; set; }
        public bool Zarva { get; set; } // 🔥 ÚJ
    }

    public class Foglalas
    {
        public int FoglalasId { get; set; }
        public int SzekrenyId { get; set; }
        public int TagId { get; set; }
        public bool Zarva { get; set; } // 🔥 FONTOS
    }
}