using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace GymFrontend.Pages
{
    public class KapcsolatModel : PageModel
    {
        [BindProperty]
        public string? Nev { get; set; }

        [BindProperty]
        [EmailAddress(ErrorMessage = "Érvénytelen e-mail cím.")]
        public string? Email { get; set; }

        [BindProperty]
        public string? Targy { get; set; }

        [BindProperty]
        public string? Uzenet { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Nev))
                ModelState.AddModelError(nameof(Nev), "Add meg a neved.");

            if (string.IsNullOrWhiteSpace(Email))
                ModelState.AddModelError(nameof(Email), "Add meg az e-mail címed.");

            if (string.IsNullOrWhiteSpace(Targy))
                ModelState.AddModelError(nameof(Targy), "Add meg a tárgyat.");

            if (string.IsNullOrWhiteSpace(Uzenet) || (Uzenet?.Length ?? 0) < 10)
                ModelState.AddModelError(nameof(Uzenet),
                    "Az üzenet legalább 10 karakter hosszú legyen.");

            if (!ModelState.IsValid)
                return Page();

            // Itt küldhetnénk valódi e-mailt vagy menthetnénk az adatbázisba.
            // Mivel ez iskolai projekt, csak egy sikerüzenetet adunk vissza.

            TempData["Sikeres"] = "Köszönjük az üzenetet! Hamarosan válaszolunk.";
            return RedirectToPage();
        }
    }
}
