using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SaaSPlatform.API.DTOs;
using SaaSPlatform.API.Services;

namespace SaaSPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET CURRENT USER
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            var email = User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
            var tenantId = User.FindFirst("TenantId")?.Value ?? string.Empty;
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

            return Ok(new
            {
                Email = email,
                TenantId = tenantId,
                Role = role
            });
        }

        // CREATE USER (Admin Only)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult CreateUser(CreateUserDto dto)
        {
            var tenantIdClaim = User.FindFirst("TenantId")?.Value;

            if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out var tenantId))
            {
                return Unauthorized("Invalid TenantId");
            }

            var user = _userService.CreateUser(dto, tenantId);

            return Ok(user);
        }

        // GET ALL USERS IN TENANT
        [HttpGet]
        public IActionResult GetUsers()
        {
            var tenantIdClaim = User.FindFirst("TenantId")?.Value;

            if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out var tenantId))
            {
                return Unauthorized("Invalid TenantId");
            }

            var users = _userService.GetUsers(tenantId);

            return Ok(users);
        }

        // DELETE USER (Admin Only)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(Guid id)
        {
            var tenantIdClaim = User.FindFirst("TenantId")?.Value;

            if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out var tenantId))
            {
                return Unauthorized("Invalid TenantId");
            }

            var deleted = _userService.DeleteUser(id, tenantId);

            if (!deleted)
                return NotFound("User not found");

            return Ok("User deleted successfully");
        }
    }
}