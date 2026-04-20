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
            var aktivBerlet = await _context.Berletek.CountAsync(b => b.Aktiv && b.VegeDatum > most);

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
                    teljesNev = b.Tag.Vezeteknev + " " + b.Tag.Keresztnev,
                    b.BelepesIdopont,
                    b.KilepesIdopont
                })
                .ToListAsync();

            return Ok(lista);
        }

        [HttpGet("napi-belepesek")]
        public async Task<IActionResult> GetNapiBelepesek()
        {
            var kezdoNap = DateTime.Today.AddDays(-6);

            var adatok = await _context.Belepesek
                .Where(b => b.BelepesIdopont.Date >= kezdoNap.Date)
                .GroupBy(b => b.BelepesIdopont.Date)
                .Select(g => new
                {
                    datum = g.Key,
                    darab = g.Count()
                })
                .ToListAsync();

            var eredmeny = Enumerable.Range(0, 7)
                .Select(i => kezdoNap.AddDays(i).Date)
                .Select(nap => new
                {
                    datum = nap.ToString("MM.dd"),
                    darab = adatok.FirstOrDefault(x => x.datum == nap)?.darab ?? 0
                })
                .ToList();

            return Ok(eredmeny);
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
                    BelepesId = b.BelepesId,
                    TagId = b.TagId,
                    TeljesNev = b.Tag.Vezeteknev + " " + b.Tag.Keresztnev,
                    BelepesIdopont = b.BelepesIdopont
                })
                .ToListAsync();

            return Ok(lista);
        }

        [HttpGet("heti-belepesek")]
        public async Task<IActionResult> GetHetiBelepesek()
        {
            var kezdoNap = DateTime.Today.AddDays(-6);

            var adatok = await _context.Belepesek
                .Where(b => b.BelepesIdopont.Date >= kezdoNap)
                .GroupBy(b => b.BelepesIdopont.Date)
                .Select(g => new
                {
                    Datum = g.Key,
                    Darab = g.Count()
                })
                .ToListAsync();

            var eredmeny = Enumerable.Range(0, 7)
                .Select(i => kezdoNap.AddDays(i))
                .Select(nap => new
                {
                    Datum = nap.ToString("MM.dd"),
                    Darab = adatok.FirstOrDefault(x => x.Datum == nap.Date)?.Darab ?? 0
                })
                .ToList();

            return Ok(eredmeny);
        }

        [HttpGet("berlet-megoszlas")]
        public async Task<IActionResult> GetBerletMegoszlas()
        {
            var most = DateTime.Now;

            var aktiv = await _context.Berletek.CountAsync(b => b.Aktiv && b.VegeDatum > most);
            var lejart = await _context.Berletek.CountAsync(b => !b.Aktiv || b.VegeDatum <= most);

            return Ok(new
            {
                Aktiv = aktiv,
                Lejart = lejart
            });
        }


    }
}