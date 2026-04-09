using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

public class DashboardModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public string Email { get; set; } = "";
    public string TeljesNev { get; set; } = "";

    public DashboardModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var token = HttpContext.Session.GetString("JWT");

        if (string.IsNullOrEmpty(token))
        {
            return RedirectToPage("/Login");
        }

        var client = _httpClientFactory.CreateClient("Api");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("api/auth/me");

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonDocument.Parse(json).RootElement;

            if (data.TryGetProperty("email", out var emailProp))
            {
                Email = emailProp.GetString() ?? "";
            }
            else
            {
                Email = "";
            }
            if(data.TryGetProperty("teljesNev", out var teljesNevProp))
            {
                TeljesNev = teljesNevProp.GetString() ?? "";
            }
            else
            {
                TeljesNev = "";
            }
            
        }

        return Page();
    }
}