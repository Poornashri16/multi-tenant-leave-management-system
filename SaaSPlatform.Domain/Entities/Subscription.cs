using System;

namespace SaaSPlatform.Domain.Entities
{
    public class Subscription
    {
        public Guid Id { get; set; }

        // Initialize string to avoid CS8618 warning
        public string PlanName { get; set; } = string.Empty;

        public Guid TenantId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}