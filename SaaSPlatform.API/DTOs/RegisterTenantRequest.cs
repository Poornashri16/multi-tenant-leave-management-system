using System.ComponentModel.DataAnnotations;

namespace SaaSPlatform.API.DTOs
{
    public class RegisterTenantRequest
    {
        [Required(ErrorMessage = "TenantName is required.")]
        public string TenantName { get; set; } = null!;

        [Required(ErrorMessage = "AdminEmail is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string AdminEmail { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = null!;
    }
}