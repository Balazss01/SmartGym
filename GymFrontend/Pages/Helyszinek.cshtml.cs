using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GymWebApiBackend.Data;
using GymWebApiBackend.Models;
using SmartGym.Models;

namespace GymFrontend.Pages
{
    public class HelyszinekModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public HelyszinekModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Helyszin> Helyszinek { get; set; } = new();

        public async Task OnGetAsync()
        {
            
            Helyszinek = await _context.Helyszinek
                .Where(h => h.Aktiv)
                .ToListAsync();
        }
    }
}