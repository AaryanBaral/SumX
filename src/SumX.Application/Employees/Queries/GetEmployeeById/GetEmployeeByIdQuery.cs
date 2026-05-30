using System;
using SumX.Application.Common.CQRS;

namespace SumX.Application.Employees.Queries.GetEmployeeById
{
    public sealed record GetEmployeeByIdQuery(Guid Id) : IQuery<EmployeeDto>;
}
