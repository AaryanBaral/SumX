using SumX.Domain.Entities;

namespace SumX.Application.User.Interface
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(Guid id);
        Task<ApplicationUser?> GetByEmailAsync(string email);

        Task<bool> ExistsByEmailAsync(string email);

        Task<Guid> CreateAsync(ApplicationUser user, string password);

        Task AssignRoleAsync(Guid userId, string role);

        Task AssignTenantAsync(Guid userId, Guid tenantId);
        Task UpdateAsync(ApplicationUser user);
        Task DeleteAsync(Guid userId);
        Task<bool> CheckPasswordAsync(string email, string password);
    }
}
