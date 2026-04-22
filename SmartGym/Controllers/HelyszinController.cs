using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymWebApiBackend.Data;
using GymWebApiBackend.Models;

namespace GymWebApiBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HelyszinController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HelyszinController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Összes aktív helyszín lekérése az adatbázisból
        /// GET /api/Helyszin
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Lekérjük az összes helyszínt, ahol az Aktiv mező true
            var aktivHelyszinek = await _context.Helyszinek
                .Where(h => h.Aktiv)
                .ToListAsync();

            return Ok(aktivHelyszinek);
        }

        /// <summary>
        /// Egy helyszín részletei ID alapján
        /// GET /api/Helyszin/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var helyszin = await _context.Helyszinek
                .FirstOrDefaultAsync(x => x.Id == id);

            if (helyszin == null)
            {
                return NotFound(new { message = "Helyszín nem található." });
            }

            return Ok(helyszin);
        }

        /// <summary>
        /// Város alapján szűrés (kis/nagybetű érzéketlen)
        /// GET /api/Helyszin/varos/{varos}
        /// </summary>
        [HttpGet("varos/{varos}")]
        public async Task<IActionResult> GetByVaros(string varos)
        {
            var lista = await _context.Helyszinek
                .Where(h => h.Aktiv && h.Varos.ToLower() == varos.ToLower())
                .ToListAsync();

            return Ok(lista);
        }
    }
}