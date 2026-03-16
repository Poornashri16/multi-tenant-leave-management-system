using SaaSPlatform.Domain.Entities;

public class Leave
{
    public Guid Id { get; set; }

    // FOREIGN KEY to User
    public Guid UserId { get; set; }

    public Guid TenantId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Reason { get; set; } = string.Empty;

    public string Status { get; set; } = "Pending";

    public string LeaveType { get; set; } = string.Empty;

    // Navigation property
    public User User { get; set; } = null!;
}