using SaaSPlatform.API.DTOs;

namespace SaaSPlatform.API.Services.Interfaces
{
    public interface IProfileService
    {
        ProfileResponseDto GetProfile(Guid userId, Guid tenantId);
        ProfileResponseDto UpdateProfile(Guid userId, Guid tenantId, UpdateProfileDto dto);
    }
}