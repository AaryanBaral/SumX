
namespace SumX.Application.Common.Abstractions.MultiTenancy
{
    public interface ITenantProvider
    {
        Guid? TenantId { get; }
        Task SetTenantAsync(Guid tenantId);
        Task<string> GetConnectionStringAsync();
    }
}
