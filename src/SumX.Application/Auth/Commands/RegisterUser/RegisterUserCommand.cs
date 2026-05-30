using System;
using MediatR;

namespace SumX.Application.Auth.Commands.RegisterUser
{
    public sealed record RegisterUserCommand(
        string Email,
        string Password,
        string Role) : IRequest<Guid>;
}
