using MediatR;

namespace SumX.Application.User.Commands.UpdateUser;

public sealed record UpdateUserCommand(
    Guid Id,
    string Email,
    string Role) : IRequest<Guid>;
