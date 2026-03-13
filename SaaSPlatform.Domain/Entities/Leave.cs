using SaaSPlatform.Domain.Entities; // if User is in the same folder/namespace
public class Leave
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }       // foreign key to User
    public Guid TenantId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";

    // NAVIGATION PROPERTY
    public User User { get; set; } = null!;
}