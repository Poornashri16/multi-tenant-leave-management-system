using SaaSPlatform.API.DTOs;
using SaaSPlatform.Persistence.Context;
using SaaSPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SaaSPlatform.API.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly AppDbContext _context;

        public LeaveService(AppDbContext context)
        {
            _context = context;
        }

        public LeaveResponseDto ApplyLeave(Guid userId, Guid tenantId, DateTime start, DateTime end, string reason, string leaveType)
        {
            var leave = new Leave
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TenantId = tenantId,
                StartDate = start,
                EndDate = end,
                Reason = reason,
                LeaveType = leaveType,
                Status = "Pending"
            };

            _context.Leaves.Add(leave);
            _context.SaveChanges();

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            return new LeaveResponseDto
            {
                Id = leave.Id,
                UserName = user?.Name ?? "Unknown",
                StartDate = leave.StartDate,
                EndDate = leave.EndDate,
                Reason = leave.Reason,
                Status = leave.Status,
                LeaveType = leave.LeaveType
            };
        }

        public List<LeaveResponseDto> GetLeaves(Guid tenantId)
        {
            return _context.Leaves
                .Include(l => l.User)
                .Where(l => l.TenantId == tenantId)
                .Select(l => new LeaveResponseDto
                {
                    Id = l.Id,
                    UserName = l.User.Name,
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                    Reason = l.Reason,
                    Status = l.Status,
                    LeaveType = l.LeaveType
                })
                .ToList();
        }

        public List<LeaveResponseDto> GetLeavesByUser(Guid userId, Guid tenantId)
        {
            return _context.Leaves
                .Include(l => l.User)
                .Where(l => l.UserId == userId && l.TenantId == tenantId)
                .Select(l => new LeaveResponseDto
                {
                    Id = l.Id,
                    UserName = l.User.Name,
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                    Reason = l.Reason,
                    Status = l.Status,
                    LeaveType = l.LeaveType
                })
                .ToList();
        }

        public List<LeaveResponseDto> GetAllLeaves(Guid tenantId, int pageNumber, int pageSize)
        {
            return _context.Leaves
                .Include(l => l.User)
                .Where(l => l.TenantId == tenantId)
                .OrderByDescending(l => l.StartDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new LeaveResponseDto
                {
                    Id = l.Id,
                    UserName = l.User.Name,
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                    Reason = l.Reason,
                    Status = l.Status,
                    LeaveType = l.LeaveType
                })
                .ToList();
        }

        public bool ApproveLeave(Guid leaveId, Guid tenantId)
        {
            var leave = _context.Leaves.FirstOrDefault(l => l.Id == leaveId && l.TenantId == tenantId);

            if (leave == null || leave.Status != "Pending")
                return false;

            var user = _context.Users.FirstOrDefault(u => u.Id == leave.UserId);

            if (user == null)
                return false;

            int days = (leave.EndDate - leave.StartDate).Days + 1;

            if (user.LeaveBalance < days)
                throw new Exception("Insufficient leave balance");

            user.LeaveBalance -= days;
            leave.Status = "Approved";

            _context.SaveChanges();

            return true;
        }

        public bool RejectLeave(Guid leaveId, Guid tenantId)
        {
            var leave = _context.Leaves.FirstOrDefault(l => l.Id == leaveId && l.TenantId == tenantId);

            if (leave == null || leave.Status != "Pending")
                return false;

            leave.Status = "Rejected";

            _context.SaveChanges();

            return true;
        }
    }
}