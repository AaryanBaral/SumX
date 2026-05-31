using MediatR;
using SumX.Application.Common.Abstractions;
using SumX.Application.Common.Abstractions.Persistence.Master;
using SumX.Application.Common.Exceptions;
using SumX.Application.User.Interface;
using SumX.Domain.Constants;

namespace SumX.Application.Tenants.Commands.DeleteTenant;

public sealed class DeleteTenantHandler : IRequestHandler<DeleteTenantCommand, Guid>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITenantDatabaseService _tenantDatabaseService;
    private readonly ICurrentUserContext _currentUserContext;

    public DeleteTenantHandler(
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

    public async Task<Guid> Handle(DeleteTenantCommand request, CancellationToken cancellationToken)
    {
        if (!string.Equals(_currentUserContext.Role, Roles.SuperAdmin, StringComparison.Ordinal))
        {
            throw new ForbiddenException("Only SuperAdmins are allowed to manage tenants.");
        }

        var tenant = await _tenantRepository.GetByIdAsync(request.Id);
        if (tenant is null)
        {
            throw new NotFoundException("Tenant not found.");
        }

        var connectionString = tenant.DatabaseConnectionString;

        await _userRepository.DeleteByTenantIdAsync(tenant.Id);
        await _tenantDatabaseService.DeleteTenantDatabaseAsync(connectionString, cancellationToken);
        await _tenantRepository.DeleteAsync(request.Id);

        return request.Id;
    }
}
