using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SmartGymAdminWPF.Services
{
    public class ApiService
    {
        private readonly HttpClient _client;
        public static string Token { get; set; } = "";

        public ApiService()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("https://localhost:7270/");

            if (!string.IsNullOrWhiteSpace(Token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Token);
            }
        }

        public async Task<string> Get(string url)
        {
            if (string.IsNullOrWhiteSpace(Token))
                throw new Exception("Nincs bejelentkezve!");

            var res = await _client.GetAsync(url);
            var content = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
                throw new Exception(content);

            return content;
        }

        public async Task<string> Post(string url, string json)
        {
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var res = await _client.PostAsync(url, content);
            var result = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
                throw new Exception(result);

            return result;
        }
    }
}