using SumX.Domain.Entities;

namespace SumX.Application.Common.Abstractions.Persistence;

public interface IApplicationUserRepository
{
    Task<bool> ExistsByEmailAsync(
        string emailAddress,
        string tenantId,
        Guid? excludedUserId,
        CancellationToken cancellationToken);

    Task<ApplicationUser?> GetEmployeeByIdAsync(
        Guid employeeId,
        string tenantId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ApplicationUser>> GetEmployeesAsync(
        string tenantId,
        CancellationToken cancellationToken);

    Task AddAsync(ApplicationUser user, CancellationToken cancellationToken);

    Task UpdateAsync(ApplicationUser user, CancellationToken cancellationToken);

    Task DeleteAsync(ApplicationUser user, CancellationToken cancellationToken);
}
