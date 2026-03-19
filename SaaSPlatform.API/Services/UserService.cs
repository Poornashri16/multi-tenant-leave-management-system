using SaaSPlatform.Domain.Entities;
using SaaSPlatform.Persistence.Context;
using SaaSPlatform.API.DTOs;
using Microsoft.Extensions.Configuration;

namespace SaaSPlatform.API.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public UserService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IEnumerable<User> GetUsersByTenant(Guid tenantId)
        {
            return _context.Users
                .Where(u => u.TenantId == tenantId)
                .ToList();
        }
        public string GetUserName(Guid userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            return user != null ? user.Name : "Unknown User";
        }

        public User? GetUserById(Guid userId)
        {
            return _context.Users.FirstOrDefault(u => u.Id == userId);
        }

        public List<UserResponseDto> GetUsers(Guid tenantId)
        {
            return _context.Users
                .Where(u => u.TenantId == tenantId)
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role
                })
                .ToList();
        }

        public UserResponseDto CreateUser(CreateUserDto dto, Guid tenantId)
        {
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
                TenantId = tenantId
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };
        }

        public bool DeleteUser(Guid id, Guid tenantId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id && u.TenantId == tenantId);

            if (user == null)
                return false;

            _context.Users.Remove(user);
            _context.SaveChanges();

            return true;
        }
        

        public (string? Token, string? Role) Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
                return (null, null);

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return (null, null);

            var claims = new List<System.Security.Claims.Claim>
            {
                new(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
                new("TenantId", user.TenantId.ToString()),
                new("Role", user.Role)
            };

            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(
                    new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
                    ),
                    Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256
                )
            );

            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(token);
            return (tokenString, user.Role);
        }
    }
}