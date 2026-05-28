using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SumX.Application.Auth.DTOs;
using SumX.Application.Auth.Interfaces;
using SumX.Application.Common.Exceptions;
using SumX.Application.User.Interface;

namespace SumX.Application.Auth.Commands.LoginUser
{
    public sealed class LoginUserHandler : IRequestHandler<LoginUserCommand, AuthResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public LoginUserHandler(
            IUserRepository userRepository,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<AuthResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var isValid = await _userRepository.CheckPasswordAsync(
                request.Email,
                request.Password);

            if (!isValid)
                throw new UnauthorizedAccessException("Invalid credentials");

            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user is null)
                throw new NotFoundException("User not found");

            var token = _jwtTokenGenerator.GenerateToken(user);

            return new AuthResult
            {
                AccessToken = token,
                UserId = user.Id,
                Email = user.Email,
                Role = user.Role,
                TenantId = user.TenantId
            };
        }
    }
}