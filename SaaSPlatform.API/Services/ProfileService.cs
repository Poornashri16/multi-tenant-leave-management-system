using SaaSPlatform.API.DTOs;              
using SaaSPlatform.Domain;                 
using SaaSPlatform.API.Services.Interfaces; 
using SaaSPlatform.Persistence.Context;   

namespace SaaSPlatform.API.Services
{
    public class ProfileService : IProfileService
    {
        private readonly AppDbContext _context;

        public ProfileService(AppDbContext context)
        {
            _context = context;
        }

        public ProfileResponseDto GetProfile(Guid userId, Guid tenantId)
        {
            var user = _context.Users
                               .FirstOrDefault(u => u.Id == userId && u.TenantId == tenantId);

            if (user == null) return null!;

            return new ProfileResponseDto
            {
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Department = user.Department,
                DateOfJoining = user.DateOfJoining,
                ProfilePictureUrl = user.ProfilePictureUrl ?? string.Empty,
                Role = user.Role
            };
        }

        // ✅ Change void to ProfileResponseDto
        public ProfileResponseDto UpdateProfile(Guid userId, Guid tenantId, UpdateProfileDto dto)
        {
            Console.WriteLine($"Updating user {userId} in tenant {tenantId}");
            Console.WriteLine($"New Name: {dto.Name}, Phone: {dto.PhoneNumber}, Dept: {dto.Department}");
            var user = _context.Users
                               .FirstOrDefault(u => u.Id == userId && u.TenantId == tenantId);

            if (user == null) return null!;

            user.Name = dto.Name;
            user.PhoneNumber = dto.PhoneNumber;
            user.Department = dto.Department;

            _context.SaveChanges();

            return new ProfileResponseDto
            {
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Department = user.Department,
                DateOfJoining = user.DateOfJoining,
                ProfilePictureUrl = user.ProfilePictureUrl ?? string.Empty,
                Role = user.Role
            };
        }
    }
}