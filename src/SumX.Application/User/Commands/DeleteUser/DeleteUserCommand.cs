using MediatR;

namespace SumX.Application.User.Commands.DeleteUser;

public sealed record DeleteUserCommand(Guid Id) : IRequest<Guid>;
