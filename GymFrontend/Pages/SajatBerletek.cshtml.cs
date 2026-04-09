using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace GymFrontend.Pages
{
    public class SajatBerletekModel : PageModel
    {
        public List<Berlet> Berletek { get; set; } = new();

        public async Task OnGetAsync()
        {
            using var client = new HttpClient();

            var token = HttpContext.Session.GetString("token");

            Console.WriteLine("TOKEN: " + token);

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            var res = await client.GetAsync("https://localhost:7270/api/Berletek");

            Console.WriteLine("STATUS: " + res.StatusCode);

            var json = await res.Content.ReadAsStringAsync();
            Console.WriteLine("JSON: " + json);

            if (!res.IsSuccessStatusCode)
                return;

            Berletek = JsonSerializer.Deserialize<List<Berlet>>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new();

        }
        public bool VanAktivBerlet => Berletek.Any(b => b.Aktiv && b.VegeDatum > DateTime.Now);
    }


    public class Berlet
    {
        public int BerletId { get; set; }
        public DateTime KezdetDatum { get; set; }
        public DateTime VegeDatum { get; set; }
        public bool Aktiv { get; set; }
        public string BerletTipusNev { get; set; }
    }
}