using System.Threading.Tasks;

namespace SumX.Application.Common.Abstractions
{
    public interface ITenantProvider
    {
        string? TenantId { get; }
        Task<string> GetConnectionStringAsync();
    }
}
