using SumX.Application.Tenants.DTOs;
using SumX.Domain.Entities.Master;

namespace SumX.Application.Tenants.Mapping;

internal static class TenantMapping
{
    public static TenantDto ToDto(this Tenant tenant) =>
        new(
            tenant.Id,
            tenant.Name,
            tenant.Email,
            tenant.TenantId,
            tenant.DatabaseConnectionString);
}
