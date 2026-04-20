using GymWebApiBackend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GymWebApiBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ErtesitesekController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ErtesitesekController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetErtesitesek()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var ertesitesek = await _context.Ertesitesek
                .Where(x => x.TagId == userId)
                .OrderByDescending(x => x.Datum)
                .Select(x => new
                {
                    x.ErtesitesId,
                    x.Uzenet,
                    x.Olvasott
                })
                .ToListAsync();

            return Ok(ertesitesek);
        }
    }
}