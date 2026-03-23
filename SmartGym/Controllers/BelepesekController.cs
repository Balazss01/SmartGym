using GymWebApiBackend.Data;
using GymWebApiBackend.DTOs;
using GymWebApiBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymWebApiBackend.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class BelepesekController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BelepesekController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var belepesek = await _context.Belepesek
                .Include(b => b.Tag)
                .ToListAsync();

            return Ok(belepesek);
        }

        [HttpGet("{id}")]
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

        [HttpPost]
        public async Task<IActionResult> Create(CreateBelepesDto dto)
        {
            var tagLetezik = await _context.Tagok.AnyAsync(t => t.TagId == dto.TagId);
            if (!tagLetezik)
            {
                return BadRequest(new { message = "A megadott tag nem létezik." });
            }

            var belepes = new Belepes
            {
                TagId = dto.TagId,
                BelepesIdopont = dto.BelepesIdopont
            };

            _context.Belepesek.Add(belepes);
            await _context.SaveChangesAsync();

            return Ok(belepes);
        }

        [HttpDelete("{id}")]
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