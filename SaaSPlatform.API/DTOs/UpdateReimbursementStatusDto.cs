using System;

// UpdateReimbursementStatusDto.cs
namespace SaaSPlatform.Application.DTOs
{
    public class UpdateReimbursementStatusDto
    {
        public string Status { get; set; } = "";
    }
}

// ReimbursementAdminDto.cs
namespace SaaSPlatform.Application.DTOs
{
    public class ReimbursementAdminDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = "";
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }
        public string EmployeeName { get; set; } = "";
    }
}

// ReimbursementDto.cs (for employee view)
namespace SaaSPlatform.Application.DTOs
{
    public class ReimbursementDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid TenantId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = "";
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }
    }
}