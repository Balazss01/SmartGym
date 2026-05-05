using GymWebApiBackend.Data;
using GymWebApiBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GymWebApiBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
            var foglalasok = await _context.SzekrenyFoglalasok.ToListAsync();
            return Ok(foglalasok);
        }

        
        [HttpPost("toggle/{szekrenyId}")]
        public async Task<IActionResult> Toggle(int szekrenyId)
        {
            var tagId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            
            var sajatFoglalas = await _context.SzekrenyFoglalasok
                .FirstOrDefaultAsync(x => x.TagId == tagId);

            
            var foglalas = await _context.SzekrenyFoglalasok
                .FirstOrDefaultAsync(x => x.SzekrenyId == szekrenyId);

            
            if (foglalas == null)
            {
                
                if (sajatFoglalas != null)
                {
                    return BadRequest("Már van lefoglalt szekrényed!");
                }

                var uj = new SzekrenyFoglalas
                {
                    TagId = tagId,
                    SzekrenyId = szekrenyId,
                    Zarva = true,
                    FoglalvaKezdete = DateTime.Now,
                    FoglalvaVege = DateTime.MinValue
                };

                _context.SzekrenyFoglalasok.Add(uj);
                await _context.SaveChangesAsync();

                return Ok("Lefoglalva");
            }

            
            if (foglalas.TagId == tagId)
            {
                foglalas.FoglalvaVege = DateTime.Now;

                _context.SzekrenyFoglalasok.Remove(foglalas);
                await _context.SaveChangesAsync();

                return Ok("Feloldva");
            }

            
            return BadRequest("Ez a szekrény már foglalt!");
        }
    }
}