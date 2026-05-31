using MediatR;
using SumX.Application.Common.Exceptions;
using SumX.Application.Common.Abstractions.Security;
using SumX.Application.Common.Abstractions.Persistence.Tenants;
using SumX.Application.Users.DTOs;
using SumX.Application.Users.Mapping;
using SumX.Domain.Constants;

namespace SumX.Application.Users.Queries.GetUsers;

public sealed class GetUsersHandler : IRequestHandler<GetUsersQuery, IReadOnlyList<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserContext _currentUserContext;

    public GetUsersHandler(
        IUserRepository userRepository,
        ICurrentUserContext currentUserContext)
    {
        _userRepository = userRepository;
        _currentUserContext = currentUserContext;
    }

    public async Task<IReadOnlyList<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        if (!string.Equals(_currentUserContext.Role, Roles.Admin, StringComparison.Ordinal))
        {
            throw new ForbiddenException("Only Admins can view tenant users.");
        }

        var tenantId = _currentUserContext.TenantId;
        if (!tenantId.HasValue)
        {
            throw new ForbiddenException("Admin user must belong to a tenant context.");
        }

        var users = await _userRepository.GetByTenantIdAsync(tenantId.Value);

        return users.Select(u => u.ToDto()).ToList();
    }
}
