using SumX.Application.Common.CQRS;

namespace SumX.Application.Employees.Queries.GetEmployeeByEmail
{
    public sealed record GetEmployeeByEmailQuery(string Email) : IQuery<EmployeeDto>;
}
