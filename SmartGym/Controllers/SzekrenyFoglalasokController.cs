using GymWebApiBackend.Data;
using GymWebApiBackend.DTOs;
using GymWebApiBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymWebApiBackend.Controllers
{
    [Authorize(Roles = "Admin,User")]
    [ApiController]
    [Route("api/[controller]")]
    public class SzekrenyFoglalasokController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SzekrenyFoglalasokController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var foglalasok = await _context.SzekrenyFoglalasok
                .Include(f => f.Tag)
                .Include(f => f.Szekreny)
                .ToListAsync();

            return Ok(foglalasok);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var foglalas = await _context.SzekrenyFoglalasok
                .Include(f => f.Tag)
                .Include(f => f.Szekreny)
                .FirstOrDefaultAsync(f => f.FoglalasId == id);

            if (foglalas == null)
            {
                return NotFound();
            }

            return Ok(foglalas);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSzekrenyFoglalasDto dto)
        {
            var tagLetezik = await _context.Tagok.AnyAsync(t => t.TagId == dto.TagId);
            if (!tagLetezik)
            {
                return BadRequest(new { message = "A megadott tag nem létezik." });
            }

            var szekrenyLetezik = await _context.Szekrenyek.AnyAsync(s => s.SzekrenyId == dto.SzekrenyId);
            if (!szekrenyLetezik)
            {
                return BadRequest(new { message = "A megadott szekrény nem létezik." });
            }

            if (dto.FoglalvaVege <= dto.FoglalvaKezdete)
            {
                return BadRequest(new { message = "A foglalás vége későbbi kell legyen, mint a kezdete." });
            }

            var utkozes = await _context.SzekrenyFoglalasok.AnyAsync(f =>
                f.SzekrenyId == dto.SzekrenyId &&
                dto.FoglalvaKezdete < f.FoglalvaVege &&
                dto.FoglalvaVege > f.FoglalvaKezdete);

            if (utkozes)
            {
                return BadRequest(new { message = "A szekrény már foglalt ebben az időszakban." });
            }

            var foglalas = new SzekrenyFoglalas
            {
                TagId = dto.TagId,
                SzekrenyId = dto.SzekrenyId,
                Zarva = dto.Zarva,
                FoglalvaKezdete = dto.FoglalvaKezdete,
                FoglalvaVege = dto.FoglalvaVege
            };

            _context.SzekrenyFoglalasok.Add(foglalas);
            await _context.SaveChangesAsync();

            return Ok(foglalas);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateSzekrenyFoglalasDto dto)
        {
            var foglalas = await _context.SzekrenyFoglalasok.FindAsync(id);

            if (foglalas == null)
            {
                return NotFound();
            }

            var tagLetezik = await _context.Tagok.AnyAsync(t => t.TagId == dto.TagId);
            if (!tagLetezik)
            {
                return BadRequest(new { message = "A megadott tag nem létezik." });
            }

            var szekrenyLetezik = await _context.Szekrenyek.AnyAsync(s => s.SzekrenyId == dto.SzekrenyId);
            if (!szekrenyLetezik)
            {
                return BadRequest(new { message = "A megadott szekrény nem létezik." });
            }

            if (dto.FoglalvaVege <= dto.FoglalvaKezdete)
            {
                return BadRequest(new { message = "A foglalás vége későbbi kell legyen, mint a kezdete." });
            }

            var utkozes = await _context.SzekrenyFoglalasok.AnyAsync(f =>
                f.SzekrenyId == dto.SzekrenyId &&
                f.FoglalasId != id &&
                dto.FoglalvaKezdete < f.FoglalvaVege &&
                dto.FoglalvaVege > f.FoglalvaKezdete);

            if (utkozes)
            {
                return BadRequest(new { message = "A szekrény már foglalt ebben az időszakban." });
            }

            foglalas.TagId = dto.TagId;
            foglalas.SzekrenyId = dto.SzekrenyId;
            foglalas.Zarva = dto.Zarva;
            foglalas.FoglalvaKezdete = dto.FoglalvaKezdete;
            foglalas.FoglalvaVege = dto.FoglalvaVege;

            await _context.SaveChangesAsync();

            return Ok(foglalas);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var foglalas = await _context.SzekrenyFoglalasok.FindAsync(id);

            if (foglalas == null)
            {
                return NotFound();
            }

            _context.SzekrenyFoglalasok.Remove(foglalas);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}