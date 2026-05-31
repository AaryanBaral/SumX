using System.Threading.Tasks;

namespace SumX.Application.Common.Abstractions.MultiTenancy
{
    public interface ITenantDatabaseService
    {
        Task CreateTenantDatabaseAsync(string connectionString, CancellationToken cancellationToken = default);

        Task DeleteTenantDatabaseAsync(string connectionString, CancellationToken cancellationToken = default);
    }
}
