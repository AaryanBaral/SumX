using System;
using SumX.Application.Common.Abstractions.CQRS;

namespace SumX.Application.Employees.Commands.DeleteEmployee
{
    public sealed record DeleteEmployeeCommand(Guid Id) : ICommand<Guid>;
}
