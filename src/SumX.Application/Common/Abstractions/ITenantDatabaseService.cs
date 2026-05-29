using System.Threading.Tasks;

namespace SumX.Application.Common.Abstractions
{
    public interface ITenantDatabaseService
    {
        Task CreateTenantDatabaseAsync(string connectionString);
    }
}
