using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartGymAdminWPF.Services
{
    public class ApiService
    {
        private readonly HttpClient client = new HttpClient();
        private const string BaseUrl = "https://localhost:7270";

        public async Task Login()
        {
            var login = new
            {
                email = "admin@smartgym.hu",
                password = "Admin123!"
            };

            var res = await client.PostAsync(
                $"{BaseUrl}/api/auth/login",
                new StringContent(
                    JsonSerializer.Serialize(login),
                    Encoding.UTF8,
                    "application/json")
            );

            var json = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
                throw new HttpRequestException("Login hiba: " + json);

            using var doc = JsonDocument.Parse(json);

            string? token = null;

            if (doc.RootElement.TryGetProperty("token", out var t1))
                token = t1.GetString();
            else if (doc.RootElement.TryGetProperty("Token", out var t2))
                token = t2.GetString();

            if (string.IsNullOrWhiteSpace(token))
                throw new HttpRequestException("Nem jött vissza token.");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<string> Get(string endpoint)
        {
            var res = await client.GetAsync($"{BaseUrl}{endpoint}");
            var json = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
                throw new HttpRequestException($"{endpoint} hiba: {json}");

            return json;
        }
        public async Task Delete(string endpoint)
        {
            var res = await client.DeleteAsync($"{BaseUrl}{endpoint}");
            var json = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
                throw new HttpRequestException($"{endpoint} törlés hiba: {json}");
        }
    }
}