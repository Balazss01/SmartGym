using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class ProfileModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ProfileModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty]
    public string Email { get; set; } = "";

    [BindProperty]
    public string Vezeteknev { get; set; } = "";

    [BindProperty]
    public string Keresztnev { get; set; } = "";

    [BindProperty]
    public DateTime SzuletesiDatum { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var token = HttpContext.Session.GetString("JWT");

        if (string.IsNullOrEmpty(token))
            return RedirectToPage("/Login");

        var client = _httpClientFactory.CreateClient("Api");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("api/auth/me");

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonDocument.Parse(json).RootElement;

            Email = data.GetProperty("email").GetString() ?? "";
            Vezeteknev = data.GetProperty("vezeteknev").GetString() ?? "";
            Keresztnev = data.GetProperty("keresztnev").GetString() ?? "";
            SzuletesiDatum = data.GetProperty("szuletesiDatum").GetDateTime();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var token = HttpContext.Session.GetString("JWT");

        if (string.IsNullOrEmpty(token))
            return RedirectToPage("/Login");

        var client = _httpClientFactory.CreateClient("Api");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var data = new
        {
            email = Email,
            vezeteknev = Vezeteknev,
            keresztnev = Keresztnev,
            szuletesiDatum = SzuletesiDatum
        };

        var content = new StringContent(
            JsonSerializer.Serialize(data),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PutAsync("api/auth/update", content);

        if (response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Mentve!");
            return Page();
        }

        var error = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError("", error);

        return Page();
    }
}