using SaaSPlatform.Domain.Entities;
using SaaSPlatform.API.DTOs;

namespace SaaSPlatform.API.Services
{
    public interface ITaskService
    {
        // Admin creates task
        Task<TaskItem> CreateTask(CreateTaskDto dto, Guid tenantId);

        // Employee gets their tasks
        List<TaskItem> GetMyTasks(Guid userId, Guid tenantId);

        // Admin gets all tasks
        List<TaskDto> GetAllTasks(Guid tenantId);

        // Employee updates task status
        Task<TaskItem?> UpdateTaskStatus(Guid taskId, Guid userId, string status);
    }
}