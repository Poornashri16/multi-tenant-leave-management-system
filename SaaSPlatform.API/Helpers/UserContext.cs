using System.Security.Claims;

namespace SaaSPlatform.API.Helpers
{
    public static class UserContext
    {
        public static Guid GetTenantId(ClaimsPrincipal user)
        {
            var tenantId = user.FindFirst("TenantId")?.Value;

            if (tenantId == null)
                throw new Exception("TenantId not found");

            return Guid.Parse(tenantId);
        }
    }
}