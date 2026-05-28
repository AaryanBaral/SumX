using System.Collections.Generic;
using System.Threading.Tasks;
using SumX.Domain.Entities.Master;

namespace SumX.Application.Common.Abstractions.Persistence.Master
{
    public interface ITenantRepository
    {
        Task<Tenant?> GetByIdAsync(string id, bool trackChanges = false);
        Task<Tenant?> GetByTenantCodeAsync(string tenantId, bool trackChanges = false);
        Task<IEnumerable<Tenant>> GetAllAsync(bool trackChanges = false);
        Task CreateAsync(Tenant tenant);
        Task UpdateAsync(Tenant tenant);
        Task DeleteAsync(string id);
        Task<bool> ExistsByCodeAsync(string tenantId);
    }
}
