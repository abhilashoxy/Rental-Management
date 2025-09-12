// Controllers/AuthController.cs
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RentalManagementService.Data;
using RentalManagementService.Models;
using RentalManagementService.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RentalManagementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest(new { message = "Email and password are required." });

            var exists = await _db.Users.AnyAsync(u => u.Email == req.Email);
            if (exists) return Conflict(new { message = "Email already registered." });

            // basic role guard
            var role = string.IsNullOrWhiteSpace(req.Role) ? "Viewer" : req.Role;
            if (role is not ("Admin" or "Manager" or "Viewer"))
                role = "Viewer";

            var user = new AppUser
            {
                Email = req.Email.Trim().ToLowerInvariant(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                Role = role
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var token = CreateJwt(user);
            return Ok(new AuthResponse(token, user.Email, user.Role));
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest req)
        {
            var email = (req.Email ?? string.Empty).Trim().ToLowerInvariant();
            var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user is null) return Unauthorized(new { message = "Invalid credentials" });

            var ok = BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash);
            if (!ok) return Unauthorized(new { message = "Invalid credentials" });

            var token = CreateJwt(user);
            return Ok(new AuthResponse(token, user.Email, user.Role));
        }

        [HttpGet("me")]
        [Authorize]
        public ActionResult<object> Me()
        {
            return Ok(new
            {
                Email = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.Identity?.Name,
                Role = User.FindFirstValue(ClaimTypes.Role)
            });
        }

        private string CreateJwt(AppUser user)
        {
            var issuer = _config["Jwt:Issuer"]!;
            var audience = _config["Jwt:Audience"]!;
            var key = _config["Jwt:Key"]!;
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(12),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
