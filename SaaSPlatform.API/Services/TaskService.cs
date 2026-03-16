using Microsoft.EntityFrameworkCore;
using SaaSPlatform.Domain.Entities;
using SaaSPlatform.API.DTOs;
using SaaSPlatform.Persistence.Context;

namespace SaaSPlatform.API.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;

        public TaskService(AppDbContext context)
        {
            _context = context;
        }

        // ADMIN → Create Task
        public async Task<TaskItem> CreateTask(CreateTaskDto dto, Guid tenantId)
        {
            var dueDateUtc = DateTime.SpecifyKind(dto.DueDate, DateTimeKind.Utc);

            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                AssignedToUserId = dto.AssignedToUserId,
                TenantId = tenantId,
                DueDate = dueDateUtc,
                Status = "Pending"
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return task;
        }

        // EMPLOYEE → Get My Tasks
        public List<TaskItem> GetMyTasks(Guid userId, Guid tenantId)
        {
            return _context.Tasks
                .Where(t => t.AssignedToUserId == userId && t.TenantId == tenantId)
                .ToList();
        }

        // ADMIN → Get All Tasks
        public List<TaskDto> GetAllTasks(Guid tenantId)
        {
            return _context.Tasks
                .Where(t => t.TenantId == tenantId)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    AssignedToUserId = t.AssignedToUserId,
                    AssignedToUserName =
                        t.AssignedToUserId == null
                        ? "Unassigned"
                        : _context.Users
                            .Where(u => u.Id == t.AssignedToUserId.Value)
                            .Select(u => u.Name)
                            .FirstOrDefault() ?? "Unassigned",
                    DueDate = t.DueDate,
                    Status = t.Status
                })
                .ToList();
        }

        // EMPLOYEE → Update Task Status
        public async Task<TaskItem?> UpdateTaskStatus(Guid taskId, Guid userId, string status)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == taskId && t.AssignedToUserId == userId);

            if (task == null)
                return null;

            task.Status = status;

            await _context.SaveChangesAsync();

            return task;
        }
    }
}