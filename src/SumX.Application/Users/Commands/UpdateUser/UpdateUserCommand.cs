using MediatR;

namespace SumX.Application.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand(
    Guid Id,
    string Email,
    string Role) : IRequest<Guid>;
