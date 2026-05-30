using MediatR;
using SumX.Application.Common.Abstractions;
using SumX.Application.Common.Exceptions;
using SumX.Application.User.Interface;
using SumX.Domain.Constants;

namespace SumX.Application.User.Command.DeleteUser;

public sealed class DeleteUserHandler : IRequestHandler<DeleteUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserContext _currentUserContext;

    public DeleteUserHandler(
        IUserRepository userRepository,
        ICurrentUserContext currentUserContext)
    {
        _userRepository = userRepository;
        _currentUserContext = currentUserContext;
    }

    public async Task<Guid> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        if (!string.Equals(_currentUserContext.Role, Roles.Admin, StringComparison.Ordinal))
        {
            throw new ForbiddenException("Only Admins can delete users.");
        }

        var adminTenantId = _currentUserContext.TenantId;
        if (!adminTenantId.HasValue)
        {
            throw new ForbiddenException("Admin user must belong to a tenant context.");
        }

        var user = await _userRepository.GetByIdAsync(request.Id);
        if (user is null)
        {
            throw new NotFoundException("User not found");
        }

        if (user.TenantId != adminTenantId)
        {
            throw new ForbiddenException("You can only delete users in your own tenant.");
        }

        await _userRepository.DeleteAsync(request.Id);

        return request.Id;
    }
}
