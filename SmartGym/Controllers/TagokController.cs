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
            var tagok = await _context.Tagok.ToListAsync();
            return Ok(tagok);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var tag = await _context.Tagok.FindAsync(id);

            if (tag == null)
                return NotFound();

            return Ok(tag);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Tag tag)
        {
            _context.Tagok.Add(tag);
            await _context.SaveChangesAsync();

            return Ok(tag);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Tag updatedTag)
        {
            var tag = await _context.Tagok.FindAsync(id);

            if (tag == null)
                return NotFound();

            tag.Vezeteknev = updatedTag.Vezeteknev;
            tag.Keresztnev = updatedTag.Keresztnev;
            tag.SzuletesiDatum = updatedTag.SzuletesiDatum;
            tag.Aktiv = updatedTag.Aktiv;

            await _context.SaveChangesAsync();

            return Ok(tag);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var tag = await _context.Tagok.FindAsync(id);

            if (tag == null)
                return NotFound();

            _context.Tagok.Remove(tag);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}   