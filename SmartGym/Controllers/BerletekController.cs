using GymWebApiBackend.Data;
using GymWebApiBackend.DTOs;
using GymWebApiBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymWebApiBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BerletekController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BerletekController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var berletek = await _context.Berletek
                .Include(b => b.Tag)
                .Include(b => b.BerletTipus)
                .ToListAsync();

            return Ok(berletek);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var berlet = await _context.Berletek
                .Include(b => b.Tag)
                .Include(b => b.BerletTipus)
                .FirstOrDefaultAsync(b => b.BerletId == id);

            if (berlet == null)
            {
                return NotFound();
            }

            return Ok(berlet);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBerletDto dto)
        {
            var tagLetezik = await _context.Tagok.AnyAsync(t => t.TagId == dto.TagId);
            if (!tagLetezik)
            {
                return BadRequest(new { message = "A megadott tag nem létezik." });
            }

            var berletTipusLetezik = await _context.BerletTipusok.AnyAsync(bt => bt.BerletTipusId == dto.BerletTipusId);
            if (!berletTipusLetezik)
            {
                return BadRequest(new { message = "A megadott bérlettípus nem létezik." });
            }

            var berlet = new Berlet
            {
                TagId = dto.TagId,
                BerletTipusId = dto.BerletTipusId,
                KezdetDatum = dto.KezdetDatum,
                VegeDatum = dto.VegeDatum,
                Aktiv = dto.Aktiv
            };

            _context.Berletek.Add(berlet);
            await _context.SaveChangesAsync();

            return Ok(berlet);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateBerletDto dto)
        {
            var berlet = await _context.Berletek.FindAsync(id);

            if (berlet == null)
            {
                return NotFound();
            }

            var tagLetezik = await _context.Tagok.AnyAsync(t => t.TagId == dto.TagId);
            if (!tagLetezik)
            {
                return BadRequest(new { message = "A megadott tag nem létezik." });
            }

            var berletTipusLetezik = await _context.BerletTipusok.AnyAsync(bt => bt.BerletTipusId == dto.BerletTipusId);
            if (!berletTipusLetezik)
            {
                return BadRequest(new { message = "A megadott bérlettípus nem létezik." });
            }

            berlet.TagId = dto.TagId;
            berlet.BerletTipusId = dto.BerletTipusId;
            berlet.KezdetDatum = dto.KezdetDatum;
            berlet.VegeDatum = dto.VegeDatum;
            berlet.Aktiv = dto.Aktiv;

            await _context.SaveChangesAsync();

            return Ok(berlet);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var berlet = await _context.Berletek.FindAsync(id);

            if (berlet == null)
            {
                return NotFound();
            }

            _context.Berletek.Remove(berlet);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}