using GymWebApiBackend.Data;
using GymWebApiBackend.DTOs;
using GymWebApiBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GymWebApiBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var letezo = await _userManager.FindByEmailAsync(dto.Email);
            if (letezo != null)
                return BadRequest(new { message = "Email már használatban" });

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                TeljesNev = dto.Vezeteknev + " " + dto.Keresztnev
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

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

            return Ok(new { message = "Sikeres regisztráció" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Unauthorized(new { message = "Hibás adatok" });

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
                return Unauthorized(new { message = "Hibás adatok" });

            var roles = await _userManager.GetRolesAsync(user);

            // 🔥 TAG LEKÉRÉS
            var tag = await _context.Tagok
                .FirstOrDefaultAsync(t => t.IdentityUserId == user.Id);

            if (tag == null)
                return BadRequest(new { message = "Nincs tag rekord" });

            // 🔥 HELYES CLAIM (INT!)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, tag.TagId.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Name, user.UserName ?? "")
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                message = "Sikeres login",
                token = tokenString,
                tagId = tag.TagId,
                email = user.Email,
                teljesNev = user.TeljesNev,
                roles = roles
            });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var tagIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(tagIdStr, out var tagId))
                return Unauthorized();

            var tag = await _context.Tagok
                .FirstOrDefaultAsync(t => t.TagId == tagId);

            if (tag == null)
                return NotFound();

            return Ok(new
            {
                tag.Vezeteknev,
                tag.Keresztnev,
                tag.SzuletesiDatum
            });
        }
    }
}