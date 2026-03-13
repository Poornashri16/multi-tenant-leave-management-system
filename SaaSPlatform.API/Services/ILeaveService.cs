using SaaSPlatform.API.DTOs;

public interface ILeaveService
{
    LeaveResponseDto ApplyLeave(Guid userId, Guid tenantId, DateTime start, DateTime end, string reason);
    List<LeaveResponseDto> GetLeaves(Guid tenantId);
    List<LeaveResponseDto> GetLeavesByUser(Guid userId, Guid tenantId);
    
    // This must match exactly:
    List<LeaveResponseDto> GetAllLeaves(Guid tenantId);

    bool ApproveLeave(Guid leaveId, Guid tenantId);
    bool RejectLeave(Guid leaveId, Guid tenantId);
}