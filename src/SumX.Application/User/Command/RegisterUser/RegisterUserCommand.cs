using System;
using MediatR;

namespace SumX.Application.User.Command.RegisterUser
{
    public sealed record RegisterUserCommand(
        string Email,
        string Password,
        string Role) : IRequest<Guid>;
}
