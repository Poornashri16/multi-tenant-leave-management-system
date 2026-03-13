using Microsoft.EntityFrameworkCore;
using SaaSPlatform.Persistence.Context;
using SaaSPlatform.Domain.Entities;

namespace SaaSPlatform.API.Services
{
    public class TenantService
    {
        private readonly AppDbContext _context;

        public TenantService(AppDbContext context)
        {
            _context = context;
        }

        // Get all tenants
        public async Task<List<Tenant>> GetAll()
        {
            return await _context.Tenants.ToListAsync();
        }

        // Get tenant by Id
        public async Task<Tenant?> GetById(Guid id)
        {
            return await _context.Tenants.FindAsync(id);
        }

        // Delete tenant by Id
        public async Task<bool> Delete(Guid id)
        {
            var tenant = await _context.Tenants.FindAsync(id);
            if (tenant == null) return false;

            _context.Tenants.Remove(tenant);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}