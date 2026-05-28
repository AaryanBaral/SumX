using MediatR;

namespace SumX.Application.User.Command.UpdateUser;

public sealed record UpdateUserCommand(
    Guid Id,
    string Email,
    string Role) : IRequest<Guid>;
