using MediatR;
using SumX.Application.Common.Abstractions;
using SumX.Application.Common.Exceptions;
using SumX.Application.User.Interface;
using SumX.Domain.Constants;

namespace SumX.Application.User.Command.UpdateUser;

public sealed class UpdateUserHandler : IRequestHandler<UpdateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserContext _currentUserContext;

    public UpdateUserHandler(
        IUserRepository userRepository,
        ICurrentUserContext currentUserContext)
    {
        _userRepository = userRepository;
        _currentUserContext = currentUserContext;
    }

    public async Task<Guid> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        if (!string.Equals(_currentUserContext.Role, Roles.Admin, StringComparison.Ordinal))
        {
            throw new ForbiddenException("Only Admins can update users.");
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
            throw new ForbiddenException("You can only update users in your own tenant.");
        }

        var existingUserWithEmail = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUserWithEmail is not null && existingUserWithEmail.Id != request.Id)
        {
            throw new AppValidationException(
                new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(UpdateUserCommand.Email),
                        $"Email address '{request.Email}' is already registered.")
                });
        }

        user.ChangeEmail(request.Email);
        user.ChangeRole(request.Role);

        await _userRepository.UpdateAsync(user);

        return user.Id;
    }
}
