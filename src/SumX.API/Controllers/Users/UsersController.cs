using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SumX.API.Models.User;
using SumX.Application.Auth.Commands.RegisterUser;
using SumX.Application.Common.Constants;
using SumX.Application.User.Commands.DeleteUser;
using SumX.Application.User.Commands.UpdateUser;

namespace SumX.API.Controllers.Users;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
[Authorize(Roles = Roles.Admin)]
public sealed class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<Guid>> Register(RegisterUserRequest request)
    {
        var command = new RegisterUserCommand(
            request.Email,
            request.Password,
            request.Role);

        var userId = await _mediator.Send(command);

        return Ok(userId);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Guid>> Update(Guid id, UpdateUserRequest request)
    {
        var command = new UpdateUserCommand(
            id,
            request.Email,
            request.Role);

        var userId = await _mediator.Send(command);

        return Ok(userId);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<Guid>> Delete(Guid id)
    {
        var command = new DeleteUserCommand(id);
        var userId = await _mediator.Send(command);

        return Ok(userId);
    }
}
