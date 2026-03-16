using System;

namespace SaaSPlatform.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }= Guid.NewGuid();

        // Initialize string properties to empty string to avoid CS8618 warnings
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public Guid TenantId { get; set; }
        public Guid RoleId { get; set; }
        public bool IsActive { get; set; }
        public string Role { get; set; } = "User";
        public int LeaveBalance { get; set; } = 20;

        public string PhoneNumber { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public DateTime DateOfJoining { get; set; }
        public string ProfilePictureUrl { get; set; } = string.Empty;
        
    }
}