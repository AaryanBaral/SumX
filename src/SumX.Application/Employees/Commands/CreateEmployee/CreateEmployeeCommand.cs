using SumX.Application.Common.CQRS;

namespace SumX.Application.Employees.Commands.CreateEmployee
{
    public sealed record CreateEmployeeCommand(
        string FullName,
        string Email) : ICommand<Guid>;
}
