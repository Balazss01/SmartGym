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

            // Megnézzük, hogy az adott szekrény foglalt-e
            var foglalas = await _context.SzekrenyFoglalasok
                .FirstOrDefaultAsync(x => x.SzekrenyId == szekrenyId);

            // Ha nincs foglalva -> foglaljuk
            if (foglalas == null)
            {
                var ujFoglalas = new SzekrenyFoglalas
                {
                    TagId = tagId,
                    SzekrenyId = szekrenyId,
                    Zarva = true,
                    FoglalvaKezdete = DateTime.Now,
                    FoglalvaVege = DateTime.MinValue
                };

                _context.SzekrenyFoglalasok.Add(ujFoglalas);
                await _context.SaveChangesAsync();

                return Ok("Szekrény lefoglalva!");
            }

            // Ha saját foglalás -> feloldás
            if (foglalas.TagId == tagId)
            {
                foglalas.Zarva = false;
                foglalas.FoglalvaVege = DateTime.Now;

                _context.SzekrenyFoglalasok.Remove(foglalas);
                await _context.SaveChangesAsync();

                return Ok("Szekrény feloldva!");
            }

            // Ha más foglalta
            return BadRequest("Ez a szekrény már foglalt!");
        }
    }
}