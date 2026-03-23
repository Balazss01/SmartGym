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
    public class BerletTipusokController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BerletTipusokController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var berletTipusok = await _context.BerletTipusok.ToListAsync();
            return Ok(berletTipusok);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var berletTipus = await _context.BerletTipusok.FindAsync(id);

            if (berletTipus == null)
            {
                return NotFound();
            }

            return Ok(berletTipus);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BerletTipus berletTipus)
        {
            _context.BerletTipusok.Add(berletTipus);
            await _context.SaveChangesAsync();

            return Ok(berletTipus);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, BerletTipus updatedBerletTipus)
        {
            var berletTipus = await _context.BerletTipusok.FindAsync(id);

            if (berletTipus == null)
            {
                return NotFound();
            }

            berletTipus.Megnevezes = updatedBerletTipus.Megnevezes;
            berletTipus.IdotartamNapok = updatedBerletTipus.IdotartamNapok;
            berletTipus.Ar = updatedBerletTipus.Ar;

            await _context.SaveChangesAsync();

            return Ok(berletTipus);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var berletTipus = await _context.BerletTipusok.FindAsync(id);

            if (berletTipus == null)
            {
                return NotFound();
            }

            _context.BerletTipusok.Remove(berletTipus);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}