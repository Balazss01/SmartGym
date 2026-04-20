using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace GymFrontend.Pages.Shared
{
    public class NotificationsPartialModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public List<ErtesitesDto> Ertesitesek { get; set; } = new();

        public NotificationsPartialModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGetAsync()
        {
            var token = HttpContext.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token))
                return;

            var client = _httpClientFactory.CreateClient("Api");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var res = await client.GetAsync("api/Ertesitesek");
            if (!res.IsSuccessStatusCode)
                return;

            var json = await res.Content.ReadAsStringAsync();

            Ertesitesek = JsonSerializer.Deserialize<List<ErtesitesDto>>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<ErtesitesDto>();
        }
    }

    public class ErtesitesDto
    {
        public int ErtesitesId { get; set; }
        public string Uzenet { get; set; }
        public bool Olvasott { get; set; }
    }
}