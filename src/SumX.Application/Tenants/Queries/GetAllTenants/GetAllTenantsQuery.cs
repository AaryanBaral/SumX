using SumX.Application.Common.Abstractions.CQRS;
using SumX.Application.Tenants.DTOs;

namespace SumX.Application.Tenants.Queries.GetAllTenants
{
    public sealed record GetAllTenantsQuery(bool TrackChanges = false) : IQuery<IReadOnlyList<TenantDto>>;
}
