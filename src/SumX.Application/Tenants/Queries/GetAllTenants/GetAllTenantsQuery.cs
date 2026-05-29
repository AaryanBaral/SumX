using System.Collections.Generic;
using SumX.Application.Common.CQRS;
using SumX.Domain.Entities.Master;

namespace SumX.Application.Tenants.Queries.GetAllTenants
{
    public sealed record GetAllTenantsQuery(bool TrackChanges = false) : IQuery<IEnumerable<Tenant>>;
}
