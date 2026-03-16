using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SaaSPlatform.API.Services;
using SaaSPlatform.API.DTOs;


[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly IUserService _userService;

    public TasksController(ITaskService taskService, IUserService userService)
    {
        _taskService = taskService;
        _userService = userService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateTask(CreateTaskDto dto)
    {
        var tenantIdClaim = User.FindFirst("TenantId")?.Value;

        if (!Guid.TryParse(tenantIdClaim, out var tenantId))
            return Unauthorized();

        var task = await _taskService.CreateTask(dto, tenantId);

        return Ok(task);
    }

    [HttpGet("my")]
    public IActionResult GetMyTasks()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var tenantIdClaim = User.FindFirst("TenantId")?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId) ||
            !Guid.TryParse(tenantIdClaim, out var tenantId))
            return Unauthorized();

        var tasks = _taskService.GetMyTasks(userId, tenantId);

        return Ok(tasks);
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetAllTasks()
    {
        var tenantIdClaim = User.FindFirst("TenantId")?.Value;

        if (!Guid.TryParse(tenantIdClaim, out var tenantId))
            return Unauthorized();

        var tasks = _taskService.GetAllTasks(tenantId);

        return Ok(tasks);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateTaskStatusDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var task = await _taskService.UpdateTaskStatus(id, userId, dto.Status);

        if (task == null)
            return NotFound();

        return Ok(task);
    }

    [HttpGet("employees")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetEmployees()
    {
        var tenantIdClaim = User.FindFirst("TenantId")?.Value;

        if (!Guid.TryParse(tenantIdClaim, out var tenantId))
            return Unauthorized();

        var employees = _userService.GetUsersByTenant(tenantId)
            .Where(u => u.Role == "Employee")
            .Select(u => new { u.Id, u.Name });

        return Ok(employees);
    }
}