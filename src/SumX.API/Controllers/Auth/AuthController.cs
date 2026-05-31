using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SumX.API.Models.Auth;
using SumX.Application.Auth.Commands.LoginUser;
using SumX.Application.Auth.DTOs;
using SumX.Application.Auth.Commands.RegisterUser;
using SumX.Application.Common.Constants;

namespace SumX.API.Controllers.Auth
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth")]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthResult>> Login(LoginRequest request)
        {
            var command = new LoginUserCommand(request.Email, request.Password);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("register")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<Guid>> Register(RegisterRequest request)
        {
            var command = new RegisterUserCommand(request.Email, request.Password, request.Role);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}