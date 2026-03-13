// SaaSPlatform.Application/DependencyInjection.cs

using Microsoft.Extensions.DependencyInjection;  // Correct namespace!

namespace SaaSPlatform.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // TODO: Register your application-level services here
            // Example:
            // services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}