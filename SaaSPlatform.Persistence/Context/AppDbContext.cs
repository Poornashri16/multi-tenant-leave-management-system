using Microsoft.EntityFrameworkCore;
using SaaSPlatform.Domain.Entities;

namespace SaaSPlatform.Persistence.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Tenant> Tenants { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Leave> Leaves { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Subscription> Subscriptions { get; set; }

        public DbSet<TaskItem> Tasks { get; set; }

        public DbSet<Reimbursement> Reimbursements { get; set; }
    }
}