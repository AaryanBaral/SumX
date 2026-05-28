using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SumX.Application.Auth.DTOs;

namespace SumX.Application.Auth.Commands.LoginUser
{
    public sealed record LoginUserCommand(
        string Email,
        string Password
    ) : IRequest<AuthResult>;
}