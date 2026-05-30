using System;
using SumX.Application.Common.CQRS;

namespace SumX.Application.Employees.Commands.DeleteEmployee
{
    public sealed record DeleteEmployeeCommand(Guid Id) : ICommand<Guid>;
}
