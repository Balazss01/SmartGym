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

        if (!response.IsSuccessStatusCode)
            return Page();

        var json = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(json))
            return Page();

        var data = JsonDocument.Parse(json).RootElement;

        if (data.TryGetProperty("email", out var email))
            Email = email.GetString() ?? "";
        else if (data.TryGetProperty("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", out var claimEmail))
            Email = claimEmail.GetString() ?? "";
        else
            Email = "";

        if (data.TryGetProperty("vezeteknev", out var vezeteknev))
            Vezeteknev = vezeteknev.GetString() ?? "";
        else
            Vezeteknev = "";

        if (data.TryGetProperty("keresztnev", out var keresztnev))
            Keresztnev = keresztnev.GetString() ?? "";
        else
            Keresztnev = "";

        if (data.TryGetProperty("szuletesiDatum", out var szuletesiDatum) &&
            szuletesiDatum.ValueKind != JsonValueKind.Null &&
            szuletesiDatum.TryGetDateTime(out var datum))
        {
            SzuletesiDatum = datum;
        }
        else
        {
            SzuletesiDatum = DateTime.Today;
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

        var body = new
        {
            email = Email,
            vezeteknev = Vezeteknev,
            keresztnev = Keresztnev,
            szuletesiDatum = SzuletesiDatum
        };

        var content = new StringContent(
            JsonSerializer.Serialize(body),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PutAsync("api/auth/update", content);

        if (response.IsSuccessStatusCode)
        {
            TempData["Siker"] = "Mentve!";
            return RedirectToPage();
        }

        var error = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError(string.Empty, string.IsNullOrWhiteSpace(error) ? "Hiba történt." : error);

        return Page();
    }
}