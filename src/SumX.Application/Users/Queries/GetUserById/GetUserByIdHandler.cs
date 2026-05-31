using MediatR;
using SumX.Application.Common.Exceptions;
using SumX.Application.Common.Abstractions.Security;
using SumX.Application.Common.Abstractions.Persistence.Tenants;
using SumX.Application.Users.DTOs;
using SumX.Application.Users.Mapping;
using SumX.Domain.Constants;

namespace SumX.Application.Users.Queries.GetUserById;

public sealed class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserContext _currentUserContext;

    public GetUserByIdHandler(
        IUserRepository userRepository,
        ICurrentUserContext currentUserContext)
    {
        _userRepository = userRepository;
        _currentUserContext = currentUserContext;
    }

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);
        if (user is null)
        {
            throw new NotFoundException("User not found");
        }

        var isOwnProfile = _currentUserContext.UserId == user.Id;
        var isAdminOfSameTenant = string.Equals(_currentUserContext.Role, Roles.Admin, StringComparison.Ordinal)
                                  && _currentUserContext.TenantId.HasValue
                                  && _currentUserContext.TenantId.Value == user.TenantId;

        var isSuperAdmin = string.Equals(_currentUserContext.Role, Roles.SuperAdmin, StringComparison.Ordinal);

        if (!isOwnProfile && !isAdminOfSameTenant && !isSuperAdmin)
        {
            throw new ForbiddenException("You are not authorized to view this user's details.");
        }

        return user.ToDto();
    }
}
