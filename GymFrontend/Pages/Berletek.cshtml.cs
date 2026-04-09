using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace GymFrontend.Pages
{
    public class BerletekModel : PageModel
    {
        public List<BerletTipus> Berletek { get; set; } = new();

        [BindProperty]
        public int Id { get; set; }

        public async Task OnGetAsync()
        {
            using var client = new HttpClient();

            var token = HttpContext.Session.GetString("token");

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            var res = await client.GetAsync("https://localhost:7270/api/BerletTipusok");

            Console.WriteLine("GET STATUS: " + res.StatusCode);

            if (!res.IsSuccessStatusCode)
                return;

            var json = await res.Content.ReadAsStringAsync();

            Berletek = JsonSerializer.Deserialize<List<BerletTipus>>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("POST LEFUTOTT");

            using var client = new HttpClient();

            var token = HttpContext.Session.GetString("token");

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }


            var body = new
            {
                berletTipusId = Id,
                kezdetDatum = DateTime.Now,
                vegeDatum = DateTime.Now.AddDays(30),
                aktiv = true
            };

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7270/api/Berletek", content);

            Console.WriteLine("POST STATUS: " + response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Sikeres vásárlás!";
            }

            return RedirectToPage();
        }
    }

    public class BerletTipus
    {
        public int BerletTipusId { get; set; }
        public string Megnevezes { get; set; }
        public int IdotartamNapok { get; set; }
        public decimal Ar { get; set; }
    }
}