using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SumX.Application.Auth.DTOs;
using SumX.Application.Common.Abstractions.Security;
using SumX.Application.Auth.Mapping;
using SumX.Application.Common.Exceptions;
using SumX.Application.Common.Abstractions.Persistence.Tenants;

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
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user is null)
                throw new NotFoundException("User not found");

            var isValid = await _userRepository.CheckPasswordAsync(
                request.Email,
                request.Password);

            if (!isValid)
                throw new UnauthorizedException("Invalid credentials");

            var token = await _jwtTokenGenerator.GenerateTokenAsync(user, cancellationToken);

            return user.ToAuthResult(token);
        }
    }
}