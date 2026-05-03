using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace GymFrontend.Pages
{
    public class FizetesModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public FizetesModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Bérlet adatok (megjelenítéshez)
        public BerletTipus? Berlet { get; set; }

        // Form mezők
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public string? KartyaTulajdonos { get; set; }

        [BindProperty]
        public string? Kartyaszam { get; set; }

        [BindProperty]
        public string? Lejarat { get; set; }

        [BindProperty]
        public string? Cvc { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = HttpContext.Session.GetString("JWT")
                     ?? HttpContext.Session.GetString("token");

            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Login");

            await BerletBetolteseAsync(token);

            if (Berlet == null)
            {
                TempData["Hiba"] = "A választott bérlet nem található.";
                return RedirectToPage("/Berletek");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = HttpContext.Session.GetString("JWT")
                     ?? HttpContext.Session.GetString("token");

            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Login");

            // Validációk
            var kartyaTiszta = (Kartyaszam ?? "").Replace(" ", "");

            if (string.IsNullOrWhiteSpace(KartyaTulajdonos))
                ModelState.AddModelError(nameof(KartyaTulajdonos), "Add meg a kártyabirtokos nevét.");

            if (kartyaTiszta.Length != 16 || !kartyaTiszta.All(char.IsDigit))
                ModelState.AddModelError(nameof(Kartyaszam), "A kártyaszám 16 számjegyből álljon.");

            if (!Regex.IsMatch(Lejarat ?? "", @"^(0[1-9]|1[0-2])\/\d{2}$"))
                ModelState.AddModelError(nameof(Lejarat), "Helytelen lejárati dátum (formátum: HH/ÉÉ).");

            if ((Cvc ?? "").Length != 3 || !(Cvc ?? "").All(char.IsDigit))
                ModelState.AddModelError(nameof(Cvc), "A CVC 3 számjegyből álljon.");

            // Bérlet újra betöltése a megjelenítéshez
            await BerletBetolteseAsync(token);

            if (!ModelState.IsValid)
                return Page();

            if (Berlet == null)
            {
                TempData["Hiba"] = "A választott bérlet nem található.";
                return RedirectToPage("/Berletek");
            }

            // Bérlet vásárlás API hívás
            var client = _httpClientFactory.CreateClient("Api");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var body = new
            {
                berletTipusId = Id,
                kezdetDatum = DateTime.Now,
                vegeDatum = DateTime.Now.AddDays(Berlet.IdotartamNapok),
                aktiv = true
            };

            var content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync("api/Berletek", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Sikeres fizetés és vásárlás!";
                return RedirectToPage("/SajatBerletek");
            }

            ModelState.AddModelError(string.Empty, "A fizetés feldolgozása sikertelen volt. Próbáld újra.");
            return Page();
        }

        private async Task BerletBetolteseAsync(string token)
        {
            var client = _httpClientFactory.CreateClient("Api");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var res = await client.GetAsync("api/BerletTipusok");
            if (!res.IsSuccessStatusCode)
                return;

            var json = await res.Content.ReadAsStringAsync();
            var berletek = JsonSerializer.Deserialize<List<BerletTipus>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<BerletTipus>();

            Berlet = berletek.FirstOrDefault(x => x.BerletTipusId == Id);
        }
    }
}