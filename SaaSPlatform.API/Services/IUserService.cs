using System;
using System.Collections.Generic;
using SaaSPlatform.Domain.Entities;
using SaaSPlatform.API.DTOs;

namespace SaaSPlatform.API.Services
{
    public interface IUserService
    {
        // Get all users of a tenant
        IEnumerable<User> GetUsersByTenant(Guid tenantId);

        // Get a single user by ID
        User? GetUserById(Guid userId);   // ✅ Changed from int to Guid

        // Get DTO list for API usage
        List<UserResponseDto> GetUsers(Guid tenantId);

        // Create user
        UserResponseDto CreateUser(CreateUserDto dto, Guid tenantId);

        // Delete user
        bool DeleteUser(Guid id, Guid tenantId);   // ✅ Changed from int to Guid

        // Login
        (string? Token, string? Role) Login(string email, string password);

        string GetUserName(Guid userId);

        
    }
}