using FluentValidation.Results;
using MediatR;
using SumX.Application.Common.Abstractions;
using SumX.Application.Common.Abstractions.MultiTenancy;
using SumX.Application.Common.Abstractions.Security;
using SumX.Application.Common.Abstractions.Persistence.Master;
using SumX.Application.Common.Abstractions.Persistence.Tenants;
using SumX.Application.Common.Exceptions;
using SumX.Domain.Constants;
using SumX.Domain.Entities;
using SumX.Domain.Entities.Master;

namespace SumX.Application.Tenants.Commands.CreateTenant;

public sealed class CreateTenantHandler : IRequestHandler<CreateTenantCommand, Guid>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITenantDatabaseService _tenantDatabaseService;
    private readonly ITenantConnectionStringBuilder _connectionStringBuilder;
    private readonly ICurrentUserContext _currentUserContext;

    public CreateTenantHandler(
        ITenantRepository tenantRepository,
        IUserRepository userRepository,
        ITenantDatabaseService tenantDatabaseService,
        ITenantConnectionStringBuilder connectionStringBuilder,
        ICurrentUserContext currentUserContext)
    {
        _tenantRepository = tenantRepository;
        _userRepository = userRepository;
        _tenantDatabaseService = tenantDatabaseService;
        _connectionStringBuilder = connectionStringBuilder;
        _currentUserContext = currentUserContext;
    }

    public async Task<Guid> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
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

        var connectionString = _connectionStringBuilder.Build(normalizedCode);

        Guid? persistedTenantId = null;
        Guid? createdAdminUserId = null;

        try
        {
            await _tenantDatabaseService.CreateTenantDatabaseAsync(connectionString, cancellationToken);

            var tenantId = Guid.NewGuid();
            var tenant = Tenant.Create(
                tenantId,
                request.Name,
                request.Email,
                normalizedCode,
                connectionString);

            await _tenantRepository.CreateAsync(tenant);
            persistedTenantId = tenantId;

            var adminUser = ApplicationUser.CreateTenantUser(
                id: Guid.NewGuid(),
                emailAddress: request.Email,
                tenantId: tenantId,
                role: Roles.Admin);

            createdAdminUserId = await _userRepository.CreateAsync(adminUser, request.AdminPassword);
            await _userRepository.AssignRoleAsync(createdAdminUserId.Value, Roles.Admin);

            return tenantId;
        }
        catch
        {
            if (createdAdminUserId.HasValue)
            {
                await _userRepository.DeleteAsync(createdAdminUserId.Value);
            }

            if (persistedTenantId.HasValue)
            {
                await _tenantRepository.DeleteAsync(persistedTenantId.Value);
            }

            await _tenantDatabaseService.DeleteTenantDatabaseAsync(connectionString, cancellationToken);
            throw;
        }
    }
}
