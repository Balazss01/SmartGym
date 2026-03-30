using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

public class RegisterModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public RegisterModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public string Vezeteknev { get; set; } = string.Empty;

    [BindProperty]
    public string Keresztnev { get; set; } = string.Empty;

    [BindProperty]
    public DateTime SzuletesiDatum { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        var client = _httpClientFactory.CreateClient("Api");

        var data = new
        {
            email = Email,
            password = Password,
            vezeteknev = Vezeteknev,
            keresztnev = Keresztnev,
            szuletesiDatum = SzuletesiDatum
        };

        var content = new StringContent(
            JsonSerializer.Serialize(data),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync("api/auth/register", content);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToPage("/Login");
        }

        var error = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError("", error);

        return Page();
    }
}