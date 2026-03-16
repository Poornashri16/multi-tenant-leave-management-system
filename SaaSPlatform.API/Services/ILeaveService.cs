using SaaSPlatform.API.DTOs;

public interface ILeaveService
{
    // Apply Leave
    LeaveResponseDto ApplyLeave(Guid userId, Guid tenantId, DateTime start, DateTime end, string reason, string leaveType);

    // Get all leaves for tenant
    List<LeaveResponseDto> GetLeaves(Guid tenantId);

    // Get leaves of a specific user
    List<LeaveResponseDto> GetLeavesByUser(Guid userId, Guid tenantId);

    // Admin paginated leave list
    List<LeaveResponseDto> GetAllLeaves(Guid tenantId, int pageNumber, int pageSize);

    // Approve leave
    bool ApproveLeave(Guid leaveId, Guid tenantId);

    // Reject leave
    bool RejectLeave(Guid leaveId, Guid tenantId);
}