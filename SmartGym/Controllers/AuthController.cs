using GymWebApiBackend.Data;
using GymWebApiBackend.DTOs;
using GymWebApiBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace GymWebApiBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var letezoFelhasznalo = await _userManager.FindByEmailAsync(dto.Email);
            if (letezoFelhasznalo != null)
            {
                return BadRequest(new { message = "Ez az email cím már használatban van." });
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                TeljesNev = dto.Vezeteknev + " " + dto.Keresztnev
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await _userManager.AddToRoleAsync(user, "User");

            var tag = new Tag
            {
                IdentityUserId = user.Id,
                Vezeteknev = dto.Vezeteknev,
                Keresztnev = dto.Keresztnev,
                SzuletesiDatum = dto.SzuletesiDatum,
                Aktiv = true
            };

            _context.Tagok.Add(tag);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Sikeres regisztráció." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Hibás email vagy jelszó." });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

            if (!result.Succeeded)
            {
                return Unauthorized(new { message = "Hibás email vagy jelszó." });
            }

            var roles = await _userManager.GetRolesAsync(user);

            var tag = await _context.Tagok
                .FirstOrDefaultAsync(t => t.IdentityUserId == user.Id);

            return Ok(new
            {
                message = "Sikeres bejelentkezés.",
                userId = user.Id,
                email = user.Email,
                teljesNev = user.TeljesNev,
                roles = roles,
                tagId = tag?.TagId
            });
        }
    }
}