using System;
using SumX.Application.Common.Abstractions.CQRS;

namespace SumX.Application.Employees.Commands.UpdateEmployee
{
    public sealed record UpdateEmployeeCommand(
        Guid Id,
        string FullName,
        string Email) : ICommand<Guid>;
}
