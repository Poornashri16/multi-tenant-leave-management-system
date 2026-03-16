namespace SaaSPlatform.API.DTOs
{
    public class UpdateTaskStatusDto
    {
        public string Status { get; set; } = null!; // Pending / In Progress / Completed
    }
}