using System;
using System.Threading.Tasks;
using SumX.Domain.Entities;

namespace SumX.Application.Common.Abstractions.Persistence.Tenants
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<ApplicationUser>> GetByTenantIdAsync(Guid tenantId);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<bool> ExistsByEmailAsync(string email);
        Task<Guid> CreateAsync(ApplicationUser user, string password);
        Task AssignRoleAsync(Guid userId, string role);
        Task AssignTenantAsync(Guid userId, Guid tenantId);
        Task UpdateAsync(ApplicationUser user);
        Task DeleteAsync(Guid userId);
        Task DeleteByTenantIdAsync(Guid tenantId);
        Task<bool> CheckPasswordAsync(string email, string password);
    }
}
