using Microsoft.Extensions.DependencyInjection;

namespace SaaSPlatform.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Register infrastructure services, e.g., repositories
            return services;
        }
    }
}