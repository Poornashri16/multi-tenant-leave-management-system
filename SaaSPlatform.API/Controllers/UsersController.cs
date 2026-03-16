using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SaaSPlatform.API.DTOs;
using SaaSPlatform.API.Services;

namespace SaaSPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // -------------------------
        // LOGIN (Anyone)
        // -------------------------
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
                return BadRequest("Email and password are required.");

            var token = _userService.Login(dto.Email, dto.Password);

            if (token == null)
                return Unauthorized("Invalid credentials");

            return Ok(new { token });
        }

        // -------------------------
        // GET CURRENT USER INFO
        // -------------------------
        [HttpGet("me")]
        [Authorize]
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

        // -------------------------
        // CREATE USER (Admin Only)
        // -------------------------
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult CreateUser([FromBody] CreateUserDto dto)
        {
            var tenantIdClaim = User.FindFirst("TenantId")?.Value;

            if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out var tenantId))
                return Unauthorized("Invalid TenantId");

            var user = _userService.CreateUser(dto, tenantId);

            return Ok(user);
        }

        // -------------------------
        // GET ALL USERS IN TENANT
        // -------------------------
        [HttpGet]
        [Authorize]
        public IActionResult GetUsers()
        {
            var tenantIdClaim = User.FindFirst("TenantId")?.Value;

            if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out var tenantId))
                return Unauthorized("Invalid TenantId");

            var users = _userService.GetUsers(tenantId);

            return Ok(users);
        }

        // -------------------------
        // DELETE USER (Admin Only)
        // -------------------------
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(Guid id)   // ✅ FIXED
        {
            var tenantIdClaim = User.FindFirst("TenantId")?.Value;

            if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out var tenantId))
                return Unauthorized("Invalid TenantId");

            var deleted = _userService.DeleteUser(id, tenantId); // ✅ FIXED

            if (!deleted)
                return NotFound("User not found");

            return Ok("User deleted successfully");
        }
    }
}