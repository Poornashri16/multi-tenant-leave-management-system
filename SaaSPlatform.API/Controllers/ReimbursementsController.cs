using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaaSPlatform.Application.DTOs;
using SaaSPlatform.Application.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SaaSPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReimbursementsController : ControllerBase
    {
        private readonly ReimbursementService _service;

        public ReimbursementsController(ReimbursementService service)
        {
            _service = service;
        }

        // Employee: Create reimbursement
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReimbursementDto dto)
        {
            if (dto == null || dto.Amount <= 0 || string.IsNullOrEmpty(dto.Description))
                return BadRequest("Amount and Description are required.");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tenantIdClaim = User.FindFirst("TenantId")?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId) || !Guid.TryParse(tenantIdClaim, out var tenantId))
                return Unauthorized("Invalid user or tenant.");

            var result = await _service.Create(userId, tenantId, dto.Amount, dto.Description);
            return Ok(result);
        }

        // Employee: Get own reimbursements
        [HttpGet("my")]
        public IActionResult MyClaims()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var tenantIdClaim = User.FindFirst("TenantId")?.Value;

                if (!Guid.TryParse(userIdClaim, out var userId) || !Guid.TryParse(tenantIdClaim, out var tenantId))
                    return Unauthorized("Invalid user or tenant.");

                var claims = _service.GetMyClaims(userId, tenantId);
                return Ok(claims);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }

        // Admin: Get all reimbursements
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public IActionResult AllClaims()
        {
            var tenantIdClaim = User.FindFirst("TenantId")?.Value;

            if (!Guid.TryParse(tenantIdClaim, out var tenantId))
                return Unauthorized("Invalid tenant.");

            var claims = _service.GetAllClaims(tenantId);
            return Ok(claims);
        }

        // Admin: Update status (generic)
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateReimbursementStatusDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Status))
                return BadRequest("Status is required.");

            var result = await _service.UpdateStatus(id, dto.Status);
            if (result == null) return NotFound("Reimbursement not found.");

            return Ok(result);
        }

        // ------------------ Admin-specific endpoints ------------------

        // Admin: Get all pending reimbursements
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PendingClaims()
        {
            try
            {
                var tenantIdClaim = User.FindFirst("TenantId")?.Value;

                if (!Guid.TryParse(tenantIdClaim, out var tenantId))
                    return Unauthorized("Invalid tenant.");

                var pending = await _service.GetPendingClaims(tenantId);

                return Ok(pending);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }

        // Admin: Approve reimbursement
        [HttpPut("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveReimbursement(Guid id)
        {
            var updated = await _service.UpdateStatus(id, "Approved");
            if (updated == null) return NotFound("Reimbursement not found.");
            return Ok(updated);
        }

        // Admin: Reject reimbursement
        [HttpPut("{id}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectReimbursement(Guid id)
        {
            var updated = await _service.UpdateStatus(id, "Rejected");
            if (updated == null) return NotFound("Reimbursement not found.");
            return Ok(updated);
        }
    }
}