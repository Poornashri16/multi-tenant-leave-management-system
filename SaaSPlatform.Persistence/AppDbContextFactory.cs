using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SaaSPlatform.Persistence.Context
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            
            // Use your connection string here
            optionsBuilder.UseNpgsql("Host=localhost;Database=MultiTenantSaaS;Username=postgres;Password=1234");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}