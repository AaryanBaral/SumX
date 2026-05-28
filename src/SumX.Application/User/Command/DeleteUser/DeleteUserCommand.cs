using System;
using MediatR;

namespace SumX.Application.User.Command.DeleteUser
{
    public sealed record DeleteUserCommand(
        Guid Id) : IRequest<Guid>;
}
