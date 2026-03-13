using SaaSPlatform.API.DTOs;
using SaaSPlatform.Persistence.Context;
using Microsoft.EntityFrameworkCore;

public class LeaveService : ILeaveService
{
    private readonly AppDbContext _context;

    public LeaveService(AppDbContext context)
    {
        _context = context;
    }

    public LeaveResponseDto ApplyLeave(Guid userId, Guid tenantId, DateTime start, DateTime end, string reason)
    {
        var leave = new Leave
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TenantId = tenantId,
            StartDate = start,
            EndDate = end,
            Reason = reason,
            Status = "Pending"
        };

        _context.Leaves.Add(leave);
        _context.SaveChanges();

        var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        return new LeaveResponseDto
        {
            Id = leave.Id,
            UserName = user != null ? user.Name : "Unknown",
            StartDate = leave.StartDate,
            EndDate = leave.EndDate,
            Reason = leave.Reason,
            Status = leave.Status
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
                UserName = l.User.Name,  // no ?. here
                StartDate = l.StartDate,
                EndDate = l.EndDate,
                Reason = l.Reason,
                Status = l.Status
            })
            .ToList();
    }

    public List<LeaveResponseDto> GetLeavesByUser(Guid userId, Guid tenantId)
    {
        return _context.Leaves
            .Include(l => l.User)
            .Where(l => l.TenantId == tenantId && l.UserId == userId)
            .Select(l => new LeaveResponseDto
            {
                Id = l.Id,
                UserName = l.User.Name,
                StartDate = l.StartDate,
                EndDate = l.EndDate,
                Reason = l.Reason,
                Status = l.Status
            })
            .ToList();
    }

    public List<LeaveResponseDto> GetAllLeaves(Guid tenantId)
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
                Status = l.Status
            })
            .ToList();
        
    }

    public bool ApproveLeave(Guid leaveId, Guid tenantId)
    {
        var leave = _context.Leaves.FirstOrDefault(l => l.Id == leaveId && l.TenantId == tenantId);
        if (leave == null || leave.Status != "Pending")
            return false;

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