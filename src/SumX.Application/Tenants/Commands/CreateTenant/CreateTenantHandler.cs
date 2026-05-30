using FluentValidation.Results;
using MediatR;
using SumX.Application.Common.Abstractions;
using SumX.Application.Common.Abstractions.Persistence.Master;
using SumX.Application.Common.Exceptions;
using SumX.Application.User.Interface;
using SumX.Domain.Constants;
using SumX.Domain.Entities;
using SumX.Domain.Entities.Master;

namespace SumX.Application.Tenants.Commands.CreateTenant
{
    public sealed class CreateTenantHandler : IRequestHandler<CreateTenantCommand, Guid>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITenantDatabaseService _tenantDatabaseService;
        private readonly ICurrentUserContext _currentUserContext;

        public CreateTenantHandler(
            ITenantRepository tenantRepository,
            IUserRepository userRepository,
            ITenantDatabaseService tenantDatabaseService,
            ICurrentUserContext currentUserContext)
        {
            _tenantRepository = tenantRepository;
            _userRepository = userRepository;
            _tenantDatabaseService = tenantDatabaseService;
            _currentUserContext = currentUserContext;
        }

        public async Task<Guid> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
        {
            // Only SuperAdmin can manage tenants
            if (!string.Equals(_currentUserContext.Role, Roles.SuperAdmin, StringComparison.Ordinal))
            {
                throw new ForbiddenException("Only SuperAdmins are allowed to manage tenants.");
            }

            var normalizedCode = request.TenantId.Trim().ToUpperInvariant();
            if (await _tenantRepository.ExistsByCodeAsync(normalizedCode))
            {
                throw new AppValidationException(
                    new[]
                    {
                        new ValidationFailure(
                            nameof(CreateTenantCommand.TenantId),
                            $"Tenant code '{request.TenantId}' is already registered.")
                    });
            }

            if (await _userRepository.ExistsByEmailAsync(request.Email))
            {
                throw new AppValidationException(
                    new[]
                    {
                        new ValidationFailure(
                            nameof(CreateTenantCommand.Email),
                            $"Email '{request.Email}' is already registered to a user.")
                    });
            }

            // 1. Setup Tenant Database dynamically
            await _tenantDatabaseService.CreateTenantDatabaseAsync(request.DatabaseConnectionString);

            // 2. Create the Tenant Entity
            var tenantId = Guid.NewGuid();
            var tenant = Tenant.Create(
                tenantId,
                request.Name,
                request.Email,
                normalizedCode,
                request.DatabaseConnectionString);

            await _tenantRepository.CreateAsync(tenant);

            // 3. Create Default Admin user for that tenant
            var adminUser = ApplicationUser.CreateTenantUser(
                id: Guid.NewGuid(),
                emailAddress: request.Email,
                tenantId: tenantId,
                role: Roles.Admin);

            var userId = await _userRepository.CreateAsync(adminUser, request.AdminPassword);
            await _userRepository.AssignRoleAsync(userId, Roles.Admin);

            return tenantId;
        }
    }
}
