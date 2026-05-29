using SumX.Application.Common.CQRS;
using SumX.Domain.Entities.Master;

namespace SumX.Application.Tenants.Queries.GetTenantById
{
    public sealed record GetTenantByIdQuery(Guid Id, bool TrackChanges = false) : IQuery<Tenant?>;
}
