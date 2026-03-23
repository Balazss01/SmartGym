using GymWebApiBackend.Data;
using GymWebApiBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymWebApiBackend.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class SzekrenyekController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SzekrenyekController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var szekrenyek = await _context.Szekrenyek.ToListAsync();
            return Ok(szekrenyek);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var szekreny = await _context.Szekrenyek.FindAsync(id);

            if (szekreny == null)
            {
                return NotFound();
            }

            return Ok(szekreny);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Szekreny szekreny)
        {
            var letezik = await _context.Szekrenyek
                .AnyAsync(s => s.SzekrenySzam == szekreny.SzekrenySzam);

            if (letezik)
            {
                return BadRequest(new { message = "Ez a szekrényszám már létezik." });
            }

            _context.Szekrenyek.Add(szekreny);
            await _context.SaveChangesAsync();

            return Ok(szekreny);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Szekreny updatedSzekreny)
        {
            var szekreny = await _context.Szekrenyek.FindAsync(id);

            if (szekreny == null)
            {
                return NotFound();
            }

            var letezik = await _context.Szekrenyek
                .AnyAsync(s => s.SzekrenySzam == updatedSzekreny.SzekrenySzam && s.SzekrenyId != id);

            if (letezik)
            {
                return BadRequest(new { message = "Ez a szekrényszám már létezik." });
            }

            szekreny.SzekrenySzam = updatedSzekreny.SzekrenySzam;
            szekreny.Aktiv = updatedSzekreny.Aktiv;

            await _context.SaveChangesAsync();

            return Ok(szekreny);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var szekreny = await _context.Szekrenyek.FindAsync(id);

            if (szekreny == null)
            {
                return NotFound();
            }

            _context.Szekrenyek.Remove(szekreny);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}