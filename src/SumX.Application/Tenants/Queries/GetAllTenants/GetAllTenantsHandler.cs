using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SumX.Application.Common.Abstractions;
using SumX.Application.Common.Abstractions.Persistence.Master;
using SumX.Application.Common.Exceptions;
using SumX.Domain.Constants;
using SumX.Application.Tenants.DTOs;
using SumX.Application.Tenants.Mapping;

namespace SumX.Application.Tenants.Queries.GetAllTenants
{
    public sealed class GetAllTenantsHandler : IRequestHandler<GetAllTenantsQuery, IReadOnlyList<TenantDto>>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ICurrentUserContext _currentUserContext;
        private readonly ITenantConnectionStringBuilder _connectionStringBuilder;

        public GetAllTenantsHandler(
            ITenantRepository tenantRepository,
            ICurrentUserContext currentUserContext,
            ITenantConnectionStringBuilder connectionStringBuilder)
        {
            _tenantRepository = tenantRepository;
            _currentUserContext = currentUserContext;
            _connectionStringBuilder = connectionStringBuilder;
        }

        public async Task<IReadOnlyList<TenantDto>> Handle(GetAllTenantsQuery request, CancellationToken cancellationToken)
        {
            if (!string.Equals(_currentUserContext.Role, Roles.SuperAdmin, StringComparison.Ordinal))
            {
                throw new ForbiddenException("Only SuperAdmins are allowed to manage tenants.");
            }

            var tenants = await _tenantRepository.GetAllAsync(request.TrackChanges);
            return tenants.Select(t => t.ToDto(_connectionStringBuilder)).ToList();
        }
    }
}
