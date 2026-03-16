using System;

namespace SaaSPlatform.API.DTOs
{
    public class CreateTaskDto
    {
        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        // Nullable because task can be unassigned
        public Guid? AssignedToUserId { get; set; }

        public string? AssignedToUserName { get; set; }

        public DateTime DueDate { get; set; }
    }
}