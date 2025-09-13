// Controllers/AuthController.cs
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RentalManagementService.Data;
using RentalManagementService.Interfaces;
using RentalManagementService.Models;
using RentalManagementService.Models.DTOs;
using RentalManagementService.Security;
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
        private readonly IEmailSender _email;
        private readonly IWebHostEnvironment _env;

        public AuthController(ApplicationDbContext db, IConfiguration config, IEmailSender email,IWebHostEnvironment env)
        {
            _db = db;
            _config = config;
            _email = email;
            _env = env;

        }
        public record ForgotPasswordRequest(string Email);
        public record ForgotPasswordResponse(string Message);
        public record ResetPasswordRequest(string Token, string NewPassword);

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
            if (user.LockoutEnabled && user.LockoutEndUtc is DateTime lo && lo > DateTime.UtcNow)
                return Unauthorized(new { message = "Account locked. Try again later." });

            var ok = BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash);
            if (!ok)
            {
                user.AccessFailedCount++;
                if (user.LockoutEnabled && user.AccessFailedCount >= 5)
                {
                    user.LockoutEndUtc = DateTime.UtcNow.AddMinutes(15); // lock for 15 mins
                    user.AccessFailedCount = 0;
                }
                await _db.SaveChangesAsync();
                return Unauthorized(new { message = "Invalid credentials." });
            }

            var token = CreateJwt(user);
            return Ok(new AuthResponse(token, user.Email, user.Role));
        }

        [HttpGet("me")]
        public async Task<ActionResult<MeDto>> Me()
        {
            var email = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? User.FindFirstValue(ClaimTypes.Email)
                        ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (email is null) return Unauthorized();

            var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
            if (user is null) return NotFound();

            return new MeDto(user.Id, user.Email, user.Name, user.Role);
        }

        [HttpPut("me")]
        public async Task<ActionResult<UpdateProfileResult>> UpdateMe([FromBody] UpdateProfileDto input)
        {
            var email = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? User.FindFirstValue(ClaimTypes.Email)
                        ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (email is null) return Unauthorized();

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user is null) return NotFound();

            // apply updates
            if (!string.IsNullOrWhiteSpace(input.Name))
                user.Name = input.Name.Trim();

            await _db.SaveChangesAsync();

            // If name affects JWT claims, return a fresh token
            var newJwt = CreateJwt(user); // uses ClaimTypes.Name
            var me = new MeDto(user.Id, user.Email, user.Name, user.Role);
            return Ok(new UpdateProfileResult(me, newJwt));
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
                 new Claim("name", user.Name ?? user.Email),
                  new Claim("role", user.Role ?? user.Email)
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
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [EnableRateLimiting("global")]
        public async Task<ActionResult<ForgotPasswordResponse>> ForgotPassword([FromBody] ForgotPasswordRequest req)
        {
            var email = (req.Email ?? "").Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(email))
                return BadRequest(new { message = "Email required" });

            var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == email);

            // Always OK to prevent user enumeration
            var okResponse = Ok(new ForgotPasswordResponse("If that email exists, a reset link was sent."));

            if (user is null) return okResponse;

            // Invalidate previous tokens
            var old = _db.PasswordResetTokens.Where(t => t.UserId == user.Id && !t.Used);
            foreach (var t in old) t.Used = true;

            // Make token & hash
            var rawToken = TokenUtil.GenerateResetToken();
            var hash = TokenUtil.HashToken(rawToken);

            var prt = new PasswordResetToken
            {
                UserId = user.Id,
                TokenHash = hash,
                ExpiresAtUtc = DateTime.UtcNow.AddHours(1),
                Used = false
            };
            _db.PasswordResetTokens.Add(prt);
            await _db.SaveChangesAsync();

            // Build reset link for frontend
            var frontend = _config["Frontend:BaseUrl"] ?? "http://localhost:4200/";
            var link = $"{frontend.TrimEnd('/')}/reset-password?token={rawToken}";

            // Email body
            var html = $@"
            <p>We received a request to reset your password.</p>
            <p><a href=""{link}"">Click here to reset your password</a></p>
            <p>This link expires in 1 hour. If you didn't request this, you can ignore this email.</p>";

            try
            {
                await _email.SendAsync(user.Email, "Reset your RentalMgmt password", html);
            }
            catch
            {
                // In dev, return the token in body to simplify testing if email fails
                if (_env.IsDevelopment())
                    return Ok(new ForgotPasswordResponse($"Dev: Use this link: {link}"));
            }

            return okResponse;
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        [EnableRateLimiting("global")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Token) || string.IsNullOrWhiteSpace(req.NewPassword))
                return BadRequest(new { message = "Token and new password are required." });

            if (!PasswordRules.IsStrong(req.NewPassword))
                return BadRequest(new { message = $"Weak password. {PasswordRules.Requirements}" });

            var hash = TokenUtil.HashToken(req.Token);
            var prt = await _db.PasswordResetTokens
                .Include(t => t.User)
                .SingleOrDefaultAsync(t => t.TokenHash == hash);

            if (prt is null || prt.Used || prt.ExpiresAtUtc < DateTime.UtcNow)
                return BadRequest(new { message = "Invalid or expired token." });

            // Update password
            prt.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
            prt.User.SecurityStamp = Guid.NewGuid().ToString("N"); // invalidate existing JWTs
            prt.Used = true;
            await _db.SaveChangesAsync();

            return Ok(new { message = "Password updated. You can now sign in." });
        }
    }

}

