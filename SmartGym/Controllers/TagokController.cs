using GymWebApiBackend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymWebApiBackend.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class TagokController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TagokController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tagok = await _context.Tagok
                .OrderBy(t => t.Vezeteknev)
                .ThenBy(t => t.Keresztnev)
                .Select(t => new
                {
                    TagId = t.TagId,
                    TeljesNev = t.Vezeteknev + " " + t.Keresztnev,
                    Vezeteknev = t.Vezeteknev,
                    Keresztnev = t.Keresztnev,
                    SzuletesiDatum = t.SzuletesiDatum,
                    Aktiv = t.Aktiv
                })
                .ToListAsync();

            return Ok(tagok);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var tag = await _context.Tagok
                .Where(t => t.TagId == id)
                .Select(t => new
                {
                    TagId = t.TagId,
                    TeljesNev = t.Vezeteknev + " " + t.Keresztnev,
                    Vezeteknev = t.Vezeteknev,
                    Keresztnev = t.Keresztnev,
                    SzuletesiDatum = t.SzuletesiDatum,
                    Aktiv = t.Aktiv
                })
                .FirstOrDefaultAsync();

            if (tag == null)
                return NotFound();

            return Ok(tag);
        }

        [HttpPut("{id}/aktiv")]
        public async Task<IActionResult> SetAktiv(int id, [FromBody] bool aktiv)
        {
            var tag = await _context.Tagok.FindAsync(id);
            if (tag == null)
                return NotFound();

            tag.Aktiv = aktiv;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Tag státusz frissítve." });
        }
    }
}