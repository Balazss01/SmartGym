using GymWebApiBackend.Data;
using GymWebApiBackend.DTOs;
using GymWebApiBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GymWebApiBackend.Controllers
{
    [Authorize(Roles = "Admin,User")]
    [ApiController]
    [Route("api/[controller]")]
    public class BelepesekController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BelepesekController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Admin: összes belépés lekérése
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var belepesek = await _context.Belepesek
                .Include(b => b.Tag)
                .OrderByDescending(b => b.BelepesIdopont)
                .ToListAsync();

            return Ok(belepesek);
        }

        // Admin: egy adott belépés lekérése
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get(int id)
        {
            var belepes = await _context.Belepesek
                .Include(b => b.Tag)
                .FirstOrDefaultAsync(b => b.BelepesId == id);

            if (belepes == null)
            {
                return NotFound();
            }

            return Ok(belepes);
        }

        // User: saját státusz lekérése
        [HttpGet("statusz")]
        public async Task<IActionResult> GetStatusz()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var aktivBelepes = await _context.Belepesek
                .FirstOrDefaultAsync(b => b.TagId == userId && b.KilepesIdopont == null);

            return Ok(new
            {
                bentVan = aktivBelepes != null,
                belepesId = aktivBelepes?.BelepesId
            });
        }

        // User: belépés a konditerembe
        [HttpPost("belep")]
        public async Task<IActionResult> Belep()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var tagLetezik = await _context.Tagok.AnyAsync(t => t.TagId == userId);
            if (!tagLetezik)
            {
                return BadRequest(new { message = "A tag nem létezik." });
            }

            // 🔥 LEJÁRT BÉRLETEK AUTOMATIKUS INAKTIVÁLÁSA
            var lejartBerletek = await _context.Berletek
                .Where(b => b.TagId == userId && b.Aktiv && b.VegeDatum <= DateTime.Now)
                .ToListAsync();

            foreach (var berlet in lejartBerletek)
            {
                berlet.Aktiv = false;
            }

            if (lejartBerletek.Any())
            {
                await _context.SaveChangesAsync();
            }

            // 🔥 VAN-E AKTÍV BÉRLET?
            var vanAktivBerlet = await _context.Berletek
                .AnyAsync(b =>
                    b.TagId == userId &&
                    b.Aktiv &&
                    b.VegeDatum > DateTime.Now);

            if (!vanAktivBerlet)
            {
                return BadRequest(new { message = "Nincs érvényes bérleted, nem léphetsz be!" });
            }

            var marBentVan = await _context.Belepesek
                .AnyAsync(b => b.TagId == userId && b.KilepesIdopont == null);

            if (marBentVan)
            {
                return BadRequest(new { message = "Már bent vagy a konditeremben." });
            }

            var belepes = new Belepes
            {
                TagId = userId,
                BelepesIdopont = DateTime.Now,
                KilepesIdopont = null
            };

            _context.Belepesek.Add(belepes);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Sikeres belépés!" });
        }

        // User: kilépés a konditeremből
        [HttpPost("kilep")]
        public async Task<IActionResult> Kilep()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var aktivBelepes = await _context.Belepesek
                .FirstOrDefaultAsync(b => b.TagId == userId && b.KilepesIdopont == null);

            if (aktivBelepes == null)
            {
                return BadRequest(new { message = "Jelenleg nem vagy bent a konditeremben." });
            }

            aktivBelepes.KilepesIdopont = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Sikeres kilépés!" });
        }

        // Admin: törlés
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var belepes = await _context.Belepesek.FindAsync(id);

            if (belepes == null)
            {
                return NotFound();
            }

            _context.Belepesek.Remove(belepes);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}