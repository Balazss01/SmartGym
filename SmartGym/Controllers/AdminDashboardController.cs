using GymWebApiBackend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymWebApiBackend.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var ma = DateTime.Today;
            var holnap = ma.AddDays(1);
            var most = DateTime.Now;

            var osszesTag = await _context.Tagok.CountAsync();
            var aktivTag = await _context.Tagok.CountAsync(t => t.Aktiv);
            var osszesBerlet = await _context.Berletek.CountAsync();

            var aktivBerlet = await _context.Berletek
                .CountAsync(b => b.Aktiv && b.KezdetDatum <= most && b.VegeDatum > most);

            var eloreMegvasaroltBerlet = await _context.Berletek
                .CountAsync(b => b.Aktiv && b.KezdetDatum > most);

            var maiBelepesek = await _context.Belepesek
                .CountAsync(b => b.BelepesIdopont >= ma && b.BelepesIdopont < holnap);

            var bentLevok = await _context.Belepesek
                .CountAsync(b => b.KilepesIdopont == null);

            var aktivSzekrenyFoglalasok = await _context.SzekrenyFoglalasok.CountAsync();

            return Ok(new
            {
                osszesTag,
                aktivTag,
                osszesBerlet,
                aktivBerlet,
                eloreMegvasaroltBerlet, 
                maiBelepesek,
                bentLevok,
                aktivSzekrenyFoglalasok
            });
        }

        [HttpGet("utolso-belepesek")]
        public async Task<IActionResult> GetUtolsoBelepesek()
        {
            var lista = await _context.Belepesek
                .Include(b => b.Tag)
                .OrderByDescending(b => b.BelepesIdopont)
                .Take(10)
                .Select(b => new
                {
                    b.BelepesId,
                    TeljesNev = b.Tag.Vezeteknev + " " + b.Tag.Keresztnev,
                    b.BelepesIdopont,
                    b.KilepesIdopont
                })
                .ToListAsync();

            return Ok(lista);
        }

        [HttpGet("bent-levok")]
        public async Task<IActionResult> GetBentLevok()
        {
            var lista = await _context.Belepesek
                .Include(b => b.Tag)
                .Where(b => b.KilepesIdopont == null)
                .OrderByDescending(b => b.BelepesIdopont)
                .Select(b => new
                {
                    b.BelepesId,
                    b.TagId,
                    TeljesNev = b.Tag.Vezeteknev + " " + b.Tag.Keresztnev,
                    b.BelepesIdopont
                })
                .ToListAsync();

            return Ok(lista);
        }

        [HttpGet("heti-belepesek")]
        public async Task<IActionResult> GetHetiBelepesek()
        {
            var kezdoNap = DateTime.Today.AddDays(-6);

            var adatok = await _context.Belepesek
                .Where(b => b.BelepesIdopont.Date >= kezdoNap.Date)
                .GroupBy(b => b.BelepesIdopont.Date)
                .Select(g => new
                {
                    Datum = g.Key,
                    Darab = g.Count()
                })
                .ToListAsync();

            var eredmeny = Enumerable.Range(0, 7)
                .Select(i => kezdoNap.AddDays(i).Date)
                .Select(nap => new
                {
                    Datum = nap.ToString("MM.dd"),
                    Darab = adatok.FirstOrDefault(x => x.Datum == nap)?.Darab ?? 0
                })
                .ToList();

            return Ok(eredmeny);
        }

        [HttpGet("berlet-megoszlas")]
        public async Task<IActionResult> GetBerletMegoszlas()
        {
            var most = DateTime.Now;

            var aktiv = await _context.Berletek
                .CountAsync(b => b.Aktiv && b.KezdetDatum <= most && b.VegeDatum > most);

            var eloreMegvasarolt = await _context.Berletek
                .CountAsync(b => b.Aktiv && b.KezdetDatum > most);

            var lejart = await _context.Berletek
                .CountAsync(b => !b.Aktiv || b.VegeDatum <= most);

            return Ok(new
            {
                Aktiv = aktiv,
                Lejart = lejart,
                EloreMegvasarolt = eloreMegvasarolt
            });
        }
    }
}