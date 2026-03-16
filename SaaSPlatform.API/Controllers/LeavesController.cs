namespace SaaSPlatform.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SaaSPlatform.API.DTOs;
using SaaSPlatform.API.Services;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LeavesController : ControllerBase
{
    private readonly ILeaveService _leaveService;

    public LeavesController(ILeaveService leaveService)
    {
        _leaveService = leaveService;
    }

    // ----------------------------
    // Apply a leave (Employee)
    // ----------------------------
    [HttpPost("apply")]
    public IActionResult ApplyLeave(ApplyLeaveDto dto)
    {
        var tenantIdClaim = User.FindFirst("TenantId")?.Value;
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(tenantIdClaim, out var tenantId))
            return Unauthorized();

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var leave = _leaveService.ApplyLeave(
            userId,
            tenantId,
            dto.StartDate,
            dto.EndDate,
            dto.Reason,
            dto.LeaveType
        );

        return Ok(leave);
    }

    // ----------------------------
    // Get all leaves for logged-in user
    // ----------------------------
    [HttpGet("my")]
    public IActionResult GetMyLeaves()
    {
        var tenantIdClaim = User.FindFirst("TenantId")?.Value;
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(tenantIdClaim, out var tenantId))
            return Unauthorized();

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var leaves = _leaveService.GetLeavesByUser(userId, tenantId);

        return Ok(leaves);
    }

    // ----------------------------
    // Get all employee leaves (Admin only)
    // ----------------------------
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetAllEmployeeLeaves([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var tenantIdClaim = User.FindFirst("TenantId")?.Value;

        if (!Guid.TryParse(tenantIdClaim, out var tenantId))
            return Unauthorized();

        var leaves = _leaveService.GetAllLeaves(tenantId, pageNumber, pageSize);

        return Ok(leaves);
    }

    // ----------------------------
    // Approve a leave (Admin only)
    // ----------------------------
    [HttpPut("{id}/approve")]
    [Authorize(Roles = "Admin")]
    public IActionResult ApproveLeave(Guid id)
    {
        var tenantIdClaim = User.FindFirst("TenantId")?.Value;

        if (!Guid.TryParse(tenantIdClaim, out var tenantId))
            return Unauthorized();

        var result = _leaveService.ApproveLeave(id, tenantId);

        if (!result)
            return BadRequest("Leave cannot be approved.");

        return Ok("Leave Approved");
    }

    // ----------------------------
    // Reject a leave (Admin only)
    // ----------------------------
    [HttpPut("{id}/reject")]
    [Authorize(Roles = "Admin")]
    public IActionResult RejectLeave(Guid id)
    {
        var tenantIdClaim = User.FindFirst("TenantId")?.Value;

        if (!Guid.TryParse(tenantIdClaim, out var tenantId))
            return Unauthorized();

        var result = _leaveService.RejectLeave(id, tenantId);

        if (!result)
            return BadRequest("Leave cannot be rejected.");

        return Ok("Leave Rejected");
    }

    // ----------------------------
    // Get Pending Leaves for logged-in user
    // ----------------------------
    [HttpGet("my-pending")]
    public IActionResult GetMyPendingLeaves()
    {
        var tenantIdClaim = User.FindFirst("TenantId")?.Value;
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(tenantIdClaim, out var tenantId))
            return Unauthorized();

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var leaves = _leaveService
            .GetLeavesByUser(userId, tenantId)
            .Where(l => l.Status == "Pending");

        return Ok(leaves);
    }
}