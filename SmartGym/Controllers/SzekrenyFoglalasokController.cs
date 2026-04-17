using GymWebApiBackend.Data;
using GymWebApiBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GymWebApiBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SzekrenyFoglalasokController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SzekrenyFoglalasokController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Összes foglalás lekérdezése
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var foglalasok = await _context.SzekrenyFoglalasok.ToListAsync();
            return Ok(foglalasok);
        }

        // Foglalás / feloldás
        [HttpPost("toggle/{szekrenyId}")]
        public async Task<IActionResult> Toggle(int szekrenyId)
        {
            var tagId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // 🔥 VAN-E MÁR FOGLALÁSA ENNEK A USERNEK?
            var sajatFoglalas = await _context.SzekrenyFoglalasok
                .FirstOrDefaultAsync(x => x.TagId == tagId);

            // 🔥 MEGNÉZZÜK A KATTINTOTT SZEKRÉNYT
            var foglalas = await _context.SzekrenyFoglalasok
                .FirstOrDefaultAsync(x => x.SzekrenyId == szekrenyId);

            // =========================
            // 1. HA NINCS FOGLALVA → FOGLALÁS
            // =========================
            if (foglalas == null)
            {
                // ❌ már van másik szekrénye
                if (sajatFoglalas != null)
                {
                    return BadRequest("Már van lefoglalt szekrényed!");
                }

                var uj = new SzekrenyFoglalas
                {
                    TagId = tagId,
                    SzekrenyId = szekrenyId,
                    Zarva = true,
                    FoglalvaKezdete = DateTime.Now,
                    FoglalvaVege = DateTime.MinValue
                };

                _context.SzekrenyFoglalasok.Add(uj);
                await _context.SaveChangesAsync();

                return Ok("Lefoglalva");
            }

            // =========================
            // 2. HA SAJÁT → FELOLDÁS
            // =========================
            if (foglalas.TagId == tagId)
            {
                foglalas.FoglalvaVege = DateTime.Now;

                _context.SzekrenyFoglalasok.Remove(foglalas);
                await _context.SaveChangesAsync();

                return Ok("Feloldva");
            }

            // =========================
            // 3. HA MÁSÉ
            // =========================
            return BadRequest("Ez a szekrény már foglalt!");
        }
    }
}