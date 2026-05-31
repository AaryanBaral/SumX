using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SumX.API.Models.User;
using SumX.Application.Auth.Commands.RegisterUser;
using SumX.Application.Common.Constants;
using SumX.Application.Users.Commands.DeleteUser;
using SumX.Application.Users.Commands.UpdateUser;
using SumX.Application.Users.Queries.GetUsers;
using SumX.Application.Users.Queries.GetUserById;
using SumX.Application.Users.DTOs;

namespace SumX.API.Controllers.Users;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
[Authorize]
public sealed class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll()
    {
        var result = await _mediator.Send(new GetUsersQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Employee},{Roles.SuperAdmin}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id));
        return Ok(result);
    }

    [HttpPost("register")]
    [Authorize(Roles = Roles.Admin)]
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
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<Guid>> Update(Guid id, UpdateUserRequest request)
    {
        var command = new UpdateUserCommand(id, request.Email, request.Role);
        var userId = await _mediator.Send(command);
        return Ok(userId);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<Guid>> Delete(Guid id)
    {
        var command = new DeleteUserCommand(id);
        var userId = await _mediator.Send(command);
        return Ok(userId);
    }
}
