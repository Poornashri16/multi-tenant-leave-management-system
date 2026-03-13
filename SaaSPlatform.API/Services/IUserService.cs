using SaaSPlatform.API.DTOs;

namespace SaaSPlatform.API.Services
{
    public interface IUserService
    {
        List<UserResponseDto> GetUsers(Guid tenantId);

        UserResponseDto CreateUser(CreateUserDto dto, Guid tenantId);

        bool DeleteUser(Guid id, Guid tenantId);
    }
}