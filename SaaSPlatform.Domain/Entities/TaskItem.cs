using SaaSPlatform.Domain.Entities;

namespace SaaSPlatform.Domain.Entities
{
    public class TaskItem
    {
        
        public Guid Id { get; set; }= Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = "Pending";
        public Guid? AssignedToUserId { get; set; }  // FK to existing User

        // Navigation property to existing User
        public User AssignedToUser { get; set; } = null!;
        public Guid TenantId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}