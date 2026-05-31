using SumX.Application.Common.Abstractions.CQRS;
using SumX.Application.Employees.DTOs;

namespace SumX.Application.Employees.Queries.GetEmployeeByEmail
{
    public sealed record GetEmployeeByEmailQuery(string Email) : IQuery<EmployeeDto>;
}
