using System;

namespace SaaSPlatform.Domain.Entities
{
    public class Role
    {
        public Guid Id { get; set; }

        // Initialize strings to avoid CS8618 warnings
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}