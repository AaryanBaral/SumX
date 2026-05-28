using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SumX.Application.Common.Abstractions.Persistence.Master;
using SumX.Domain.Entities.Master;

namespace SumX.Infrastructure.Persistence.Master.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private readonly MasterDbContext _context;

        public TenantRepository(MasterDbContext context)
        {
            _context = context;
        }

        public async Task<Tenant?> GetByIdAsync(string id, bool trackChanges = false)
        {
            return trackChanges
                ? await _context.Tenants.FirstOrDefaultAsync(t => t.Id == id)
                : await _context.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Tenant?> GetByTenantCodeAsync(string tenantId, bool trackChanges = false)
        {
            return trackChanges
                ? await _context.Tenants.FirstOrDefaultAsync(t => t.TenantId == tenantId)
                : await _context.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.TenantId == tenantId);
        }

        public async Task<IEnumerable<Tenant>> GetAllAsync(bool trackChanges = false)
        {
            return trackChanges
                ? await _context.Tenants.ToListAsync()
                : await _context.Tenants.AsNoTracking().ToListAsync();
        }

        public async Task CreateAsync(Tenant tenant)
        {
            await _context.Tenants.AddAsync(tenant);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Tenant tenant)
        {
            _context.Tenants.Update(tenant);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var tenant = await _context.Tenants.FindAsync(id);
            if (tenant != null)
            {
                _context.Tenants.Remove(tenant);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsByCodeAsync(string tenantId)
        {
            return await _context.Tenants.AnyAsync(t => t.TenantId == tenantId);
        }
    }
}
