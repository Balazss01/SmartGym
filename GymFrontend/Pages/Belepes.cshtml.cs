using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace GymFrontend.Pages
{
    public class BelepesModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BelepesModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public bool BentVan { get; set; }

        public async Task OnGetAsync()
        {
            var token = HttpContext.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token))
                return;

            var client = _httpClientFactory.CreateClient("Api");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var res = await client.GetAsync("api/Belepesek/statusz");

            if (!res.IsSuccessStatusCode)
                return;

            var json = await res.Content.ReadAsStringAsync();
            var data = JsonDocument.Parse(json).RootElement;

            if (data.TryGetProperty("bentVan", out var bent))
                BentVan = bent.GetBoolean();
        }

        public async Task<IActionResult> OnPostBelepAsync()
        {
            var token = HttpContext.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Login");

            var client = _httpClientFactory.CreateClient("Api");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var res = await client.PostAsync("api/Belepesek/belep", null);

            if (res.IsSuccessStatusCode)
                TempData["Siker"] = "Sikeres belépés!";
            else
                TempData["Hiba"] = await res.Content.ReadAsStringAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostKilepAsync()
        {
            var token = HttpContext.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Login");

            var client = _httpClientFactory.CreateClient("Api");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var res = await client.PostAsync("api/Belepesek/kilep", null);

            if (res.IsSuccessStatusCode)
                TempData["Siker"] = "Sikeres kilépés!";
            else
                TempData["Hiba"] = await res.Content.ReadAsStringAsync();

            return RedirectToPage();
        }
    }
}