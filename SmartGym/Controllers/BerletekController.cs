using GymWebApiBackend.Data;
using GymWebApiBackend.DTOs;
using GymWebApiBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GymWebApiBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BerletekController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BerletekController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ── DTOs ────────────────────────────────────────────────────

        public class BerletListDto
        {
            public int BerletId { get; set; }
            public int TagId { get; set; }          
            public string TeljesNev { get; set; } = "";    
            public DateTime KezdetDatum { get; set; }
            public DateTime VegeDatum { get; set; }
            public bool Aktiv { get; set; }
            public string BerletTipusNev { get; set; } = "";
            public int BerletTipusId { get; set; }          
        }


        [HttpGet("admin-all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var berletek = await _context.Berletek
                .Include(b => b.Tag)
                .Include(b => b.BerletTipus)
                .OrderByDescending(b => b.KezdetDatum)
                .Select(b => new BerletListDto
                {
                    BerletId = b.BerletId,
                    TagId = b.TagId,
                    TeljesNev = b.Tag.Vezeteknev + " " + b.Tag.Keresztnev,
                    KezdetDatum = b.KezdetDatum,
                    VegeDatum = b.VegeDatum,
                    Aktiv = b.Aktiv,
                    BerletTipusNev = b.BerletTipus.Megnevezes,
                    BerletTipusId = b.BerletTipusId
                })
                .ToListAsync();

            return Ok(berletek);
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var berletek = await _context.Berletek
                .Where(b => b.TagId == userId)
                .Include(b => b.BerletTipus)
                .OrderByDescending(b => b.KezdetDatum)
                .Select(b => new BerletListDto
                {
                    BerletId = b.BerletId,
                    TagId = b.TagId,
                    TeljesNev = b.Tag.Vezeteknev + " " + b.Tag.Keresztnev,
                    KezdetDatum = b.KezdetDatum,
                    VegeDatum = b.VegeDatum,
                    Aktiv = b.Aktiv,
                    BerletTipusNev = b.BerletTipus.Megnevezes,
                    BerletTipusId = b.BerletTipusId
                })
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
                return NotFound();

            return Ok(berlet);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBerletDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var tipus = await _context.BerletTipusok
                .FirstOrDefaultAsync(t => t.BerletTipusId == dto.BerletTipusId);

            if (tipus == null)
                return BadRequest(new { message = "Bérlettípus nem létezik" });

            var most = DateTime.Now;

            var utolsoBerlet = await _context.Berletek
                .Where(b => b.TagId == userId && b.Aktiv && b.VegeDatum > most)
                .OrderByDescending(b => b.VegeDatum)
                .FirstOrDefaultAsync();

            var kezdet = utolsoBerlet != null ? utolsoBerlet.VegeDatum : most;
            var vege = kezdet.AddDays(tipus.IdotartamNapok);

            var berlet = new Berlet
            {
                TagId = userId,
                BerletTipusId = dto.BerletTipusId,
                KezdetDatum = kezdet,
                VegeDatum = vege,
                Aktiv = true
            };

            _context.Berletek.Add(berlet);

           

            await _context.SaveChangesAsync();

            return Ok(new { message = "Bérlet sikeresen létrehozva", kezdet, vege });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateBerletDto dto)
        {
            var berlet = await _context.Berletek.FindAsync(id);

            if (berlet == null)
                return NotFound();

            if (dto.BerletTipusId != 0)
            {
                var berletTipusLetezik = await _context.BerletTipusok
                    .AnyAsync(bt => bt.BerletTipusId == dto.BerletTipusId);

                if (!berletTipusLetezik)
                    return BadRequest(new { message = "A megadott bérlettípus nem létezik." });

                berlet.BerletTipusId = dto.BerletTipusId;
            }

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
                return NotFound();

            _context.Berletek.Remove(berlet);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}