using SumX.Application.Common.Abstractions.CQRS;
using SumX.Application.Tenants.DTOs;

namespace SumX.Application.Tenants.Queries.GetTenantById
{
    public sealed record GetTenantByIdQuery(Guid Id, bool TrackChanges = false) : IQuery<TenantDto>;
}
