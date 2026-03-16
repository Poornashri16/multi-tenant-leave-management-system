using SaaSPlatform.Application.DTOs;
using SaaSPlatform.Persistence.Context;
using SaaSPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SaaSPlatform.API.Services;  // <--- this allows access to IUserService

namespace SaaSPlatform.Application.Services
{
    public class ReimbursementService
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;

        public ReimbursementService(AppDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        // Create reimbursement (Employee)
        public async Task<ReimbursementDto> Create(Guid userId, Guid tenantId, decimal amount, string description)
        {
            var reimbursement = new Reimbursement
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TenantId = tenantId,
                Amount = amount,
                Description = description,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.Reimbursements.Add(reimbursement);
            await _context.SaveChangesAsync();

            return new ReimbursementDto
            {
                Id = reimbursement.Id,
                UserId = reimbursement.UserId,
                TenantId = reimbursement.TenantId,
                Amount = reimbursement.Amount,
                Description = reimbursement.Description,
                Status = reimbursement.Status,
                CreatedAt = reimbursement.CreatedAt
            };
        }

        // Get reimbursements for the logged-in employee
        public List<ReimbursementDto> GetMyClaims(Guid userId, Guid tenantId)
        {
            return _context.Reimbursements
                           .Where(r => r.UserId == userId && r.TenantId == tenantId)
                           .OrderByDescending(r => r.CreatedAt)
                           .Select(r => new ReimbursementDto
                           {
                               Id = r.Id,
                               UserId = r.UserId,
                               TenantId = r.TenantId,
                               Amount = r.Amount,
                               Description = r.Description,
                               Status = r.Status,
                               CreatedAt = r.CreatedAt
                           })
                           .ToList();
        }

        // Get all reimbursements for admin in a tenant
        public List<ReimbursementAdminDto> GetAllClaims(Guid tenantId)
        {
            return _context.Reimbursements
                           .Where(r => r.TenantId == tenantId)
                           .OrderByDescending(r => r.CreatedAt)
                           .Select(r => new ReimbursementAdminDto
                           {
                               Id = r.Id,
                               UserId = r.UserId,
                               EmployeeName = _userService.GetUserName(r.UserId), 
                               Amount = r.Amount,
                               Description = r.Description,
                               Status = r.Status, 
                               CreatedAt = r.CreatedAt
                           })
                           .ToList();
        }
        public async Task<List<Reimbursement>> GetPendingClaims(Guid tenantId)
        {
            return await _context.Reimbursements
                .Where(r => r.TenantId == tenantId && r.Status == "Pending")
                .ToListAsync();
        }

        // Update reimbursement status (Admin)
        public async Task<ReimbursementDto?> UpdateStatus(Guid id, string status)
        {
            var reimbursement = await _context.Reimbursements.FindAsync(id);
            if (reimbursement == null) return null;

            reimbursement.Status = status;
            await _context.SaveChangesAsync();

            return new ReimbursementDto
            {
                Id = reimbursement.Id,
                UserId = reimbursement.UserId,
                TenantId = reimbursement.TenantId,
                Amount = reimbursement.Amount,
                Description = reimbursement.Description,
                Status = reimbursement.Status,
                CreatedAt = reimbursement.CreatedAt
            };
        }
    }
}