using MediatR;
using Microsoft.AspNetCore.Mvc;
using SumX.Application.Auth.Commands.LoginUser;
using SumX.Application.Auth.DTOs;

namespace SumX.API.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResult>> Login(LoginUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}