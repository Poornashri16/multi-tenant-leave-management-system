using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SaaSPlatform.API.DTOs;
using SaaSPlatform.API.Services;
using SaaSPlatform.API.Services.Interfaces; // <-- IProfileService lives here
            

namespace SaaSPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet("me")]
        public IActionResult GetMyProfile()
        {
            var tenantId = Guid.Parse(User.FindFirst("TenantId")?.Value);
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var profile = _profileService.GetProfile(userId, tenantId);
            return Ok(profile);
        }

        [HttpPut("me")]
        public IActionResult UpdateMyProfile(UpdateProfileDto dto)
        {
            var tenantId = Guid.Parse(User.FindFirst("TenantId")?.Value ?? throw new InvalidOperationException("TenantId missing"));
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("UserId missing"));


            var updatedProfile = _profileService.UpdateProfile(userId, tenantId, dto);
            
            if (updatedProfile == null)
                return BadRequest("Profile update failed.");

            return Ok(updatedProfile);
        }
    }
}