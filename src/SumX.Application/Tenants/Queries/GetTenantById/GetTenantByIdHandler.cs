using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SumX.Application.Common.Abstractions;
using SumX.Application.Common.Abstractions.MultiTenancy;
using SumX.Application.Common.Abstractions.Security;
using SumX.Application.Common.Abstractions.Persistence.Master;
using SumX.Application.Common.Exceptions;
using SumX.Domain.Constants;
using SumX.Application.Tenants.DTOs;
using SumX.Application.Tenants.Mapping;

namespace SumX.Application.Tenants.Queries.GetTenantById
{
    public sealed class GetTenantByIdHandler : IRequestHandler<GetTenantByIdQuery, TenantDto>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ICurrentUserContext _currentUserContext;
        private readonly ITenantConnectionStringBuilder _connectionStringBuilder;

        public GetTenantByIdHandler(
            ITenantRepository tenantRepository,
            ICurrentUserContext currentUserContext,
            ITenantConnectionStringBuilder connectionStringBuilder)
        {
            _tenantRepository = tenantRepository;
            _currentUserContext = currentUserContext;
            _connectionStringBuilder = connectionStringBuilder;
        }

        public async Task<TenantDto> Handle(GetTenantByIdQuery request, CancellationToken cancellationToken)
        {
            if (!string.Equals(_currentUserContext.Role, Roles.SuperAdmin, StringComparison.Ordinal))
            {
                throw new ForbiddenException("Only SuperAdmins are allowed to manage tenants.");
            }

            var tenant = await _tenantRepository.GetByIdAsync(request.Id, request.TrackChanges);
            if (tenant is null)
            {
                throw new NotFoundException("Tenant not found.");
            }

            return tenant.ToDto(_connectionStringBuilder);
        }
    }
}
