using System.Security.Claims;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SumX.API.Models.Employee;
using SumX.Application.Common.Constants;
using SumX.Application.Common.Exceptions;
using SumX.Application.Employees.Commands.CreateEmployee;
using SumX.Application.Employees.Commands.DeleteEmployee;
using SumX.Application.Employees.Commands.UpdateEmployee;
using SumX.Application.Employees.DTOs;
using SumX.Application.Employees.Queries;
using SumX.Application.Employees.Queries.GetAllEmployees;
using SumX.Application.Employees.Queries.GetEmployeeByEmail;
using SumX.Application.Employees.Queries.GetEmployeeById;

namespace SumX.API.Controllers.Employees;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/employees")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Employee}")]
public sealed class EmployeesController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmployeesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<Guid>> Create(CreateEmployeeRequest request)
    {
        var command = new CreateEmployeeCommand(request.FullName, request.Email);
        var id = await _mediator.Send(command);
        return Ok(id);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<Guid>> Update(Guid id, UpdateEmployeeRequest request)
    {
        var command = new UpdateEmployeeCommand(id, request.FullName, request.Email);
        var updatedId = await _mediator.Send(command);
        return Ok(updatedId);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<Guid>> Delete(Guid id)
    {
        var command = new DeleteEmployeeCommand(id);
        var deletedId = await _mediator.Send(command);
        return Ok(deletedId);
    }

    [HttpGet("me")]
    [Authorize(Roles = Roles.Employee)]
    public async Task<ActionResult<EmployeeDto>> GetMe()
    {
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;
        if (string.IsNullOrEmpty(userEmail))
        {
            throw new AppValidationException(
                new[] { new ValidationFailure("Email", "User email claim is missing.") });
        }

        var query = new GetEmployeeByEmailQuery(userEmail);
        var employee = await _mediator.Send(query);
        return Ok(employee);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<EmployeeDto>> GetById(Guid id)
    {
        var query = new GetEmployeeByIdQuery(id);
        var employee = await _mediator.Send(query);
        return Ok(employee);
    }

    [HttpGet]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll()
    {
        var query = new GetAllEmployeesQuery();
        var employees = await _mediator.Send(query);
        return Ok(employees);
    }
}
