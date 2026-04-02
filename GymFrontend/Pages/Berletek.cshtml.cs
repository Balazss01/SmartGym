using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

            var res = await client.GetAsync("https://localhost:7270/api/BerletTipusok");

            if (!res.IsSuccessStatusCode)
                return;

            var json = await res.Content.ReadAsStringAsync();

            Berletek = JsonSerializer.Deserialize<List<BerletTipus>>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new();
        }

        public async Task OnPostAsync()
        {
            using var client = new HttpClient();

            var body = new
            {
                berletTipusId = Id,
                vasarlasDatum = DateTime.Now
            };

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7270/api/Berletek", content);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Hiba a vásárlásnál");
            }

            await OnGetAsync();
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