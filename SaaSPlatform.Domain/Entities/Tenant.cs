namespace SaaSPlatform.Domain.Entities
{
    public class Tenant
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // ⚡ Make sure this exists
        public string Name { get; set; } = null!;

        // Optional: Add other properties
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}