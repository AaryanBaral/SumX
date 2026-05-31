using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SumX.Application.Common.Abstractions;
using SumX.Application.Common.Abstractions.Persistence.Master;
using SumX.Application.Common.Exceptions;
using SumX.Domain.Constants;

namespace SumX.Application.Tenants.Commands.UpdateTenant
{
    public sealed class UpdateTenantHandler : IRequestHandler<UpdateTenantCommand, Guid>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ICurrentUserContext _currentUserContext;

        public UpdateTenantHandler(
            ITenantRepository tenantRepository,
            ICurrentUserContext currentUserContext)
        {
            _tenantRepository = tenantRepository;
            _currentUserContext = currentUserContext;
        }

        public async Task<Guid> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
        {
            if (!string.Equals(_currentUserContext.Role, Roles.SuperAdmin, StringComparison.Ordinal))
            {
                throw new ForbiddenException("Only SuperAdmins are allowed to manage tenants.");
            }

            var tenant = await _tenantRepository.GetByIdAsync(request.Id, trackChanges: true);
            if (tenant is null)
            {
                throw new NotFoundException("Tenant not found.");
            }

            tenant.Rename(request.Name);
            tenant.ChangeEmail(request.Email);

            await _tenantRepository.UpdateAsync(tenant);

            return tenant.Id;
        }
    }
}
