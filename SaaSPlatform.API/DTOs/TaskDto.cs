using System;

namespace SaaSPlatform.API.DTOs
{
    public class TaskDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public Guid? AssignedToUserId { get; set; }

        public string? AssignedToUserName { get; set; }

        public DateTime DueDate { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}