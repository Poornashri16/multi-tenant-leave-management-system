using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SaaSPlatform.API.DTOs;
using SaaSPlatform.Domain.Entities;
using SaaSPlatform.Persistence.Context;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SaaSPlatform.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // ===============================
        // REGISTER TENANT + ADMIN USER
        // ===============================
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterTenantRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.AdminEmail))
                return BadRequest(new { message = "Email already exists" });

            var tenant = new Tenant { Name = request.TenantName };
            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();

            var user = new User
            {
                Name = "Admin",
                Email = request.AdminEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                TenantId = tenant.Id,
                Role = "Admin"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Tenant registered successfully",
                tenantId = tenant.Id,
                adminEmail = user.Email
            });
        }

        // ===============================
        // LOGIN USER
        // ===============================
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid email or password" });

            // JWT claims
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("TenantId", user.TenantId.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key not configured"))
            );
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            var response = new LoginResponse
            {
                Token = jwt,
                Name = user.Name,
                TenantId = user.TenantId,
                Role = user.Role 
            };

            return Ok(response);
        }
    }
}