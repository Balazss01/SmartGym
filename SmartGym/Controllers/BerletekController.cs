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
        public class BerletListDto
        {
            public int BerletId { get; set; }
            public DateTime KezdetDatum { get; set; }
            public DateTime VegeDatum { get; set; }
            public bool Aktiv { get; set; }
            public string BerletTipusNev { get; set; }
        }
        // SAJÁT BÉRLETEK (frontend ezt hívja)
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var berletek = await _context.Berletek
                .Where(b => b.TagId == userId)
                .Include(b => b.BerletTipus)
                .Select(b => new BerletListDto
                {
                    BerletId = b.BerletId,
                    KezdetDatum = b.KezdetDatum,
                    VegeDatum = b.VegeDatum,
                    Aktiv = b.Aktiv,
                    BerletTipusNev = b.BerletTipus.Megnevezes
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
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var tipus = await _context.BerletTipusok
                .FirstOrDefaultAsync(t => t.BerletTipusId == dto.BerletTipusId);

            if (tipus == null)
                return BadRequest(new { message = "Bérlettípus nem létezik" });

            // 🔥 AKTÍV bérlet keresése
            var aktivBerlet = await _context.Berletek
                .Where(b => b.TagId == userId && b.Aktiv && b.VegeDatum > DateTime.Now)
                .OrderByDescending(b => b.VegeDatum)
                .FirstOrDefaultAsync();

            

            DateTime kezdet;

            if (aktivBerlet != null)
            {
                // 👉 STACKELÉS: az új bérlet a régi után indul
                kezdet = aktivBerlet.VegeDatum;
            }
            else
            {
                kezdet = DateTime.Now;
            }

            var vege = kezdet.AddDays(tipus.IdotartamNapok);

            // 🔥 régi marad aktív (NEM kapcsoljuk ki!)
            var berlet = new Berlet
            {
                TagId = userId,
                BerletTipusId = dto.BerletTipusId,
                KezdetDatum = kezdet,
                VegeDatum = vege,
                Aktiv = true
            };

            _context.Berletek.Add(berlet);

            _context.Ertesitesek.Add(new Ertesites
            {
                TagId = userId,
                Uzenet = "Új bérlet hozzáadva (stackelve)!",
                Olvasott = false,
                Datum = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(new { message = "Bérlet létrehozva" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateBerletDto dto)
        {
            var berlet = await _context.Berletek.FindAsync(id);

            if (berlet == null)
                return NotFound();

            var berletTipusLetezik = await _context.BerletTipusok
                .AnyAsync(bt => bt.BerletTipusId == dto.BerletTipusId);

            if (!berletTipusLetezik)
            {
                return BadRequest(new { message = "A megadott bérlettípus nem létezik." });
            }

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
                return NotFound();

            _context.Berletek.Remove(berlet);
            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}