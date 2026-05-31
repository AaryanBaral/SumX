using System;
using SumX.Application.Common.Abstractions.CQRS;
using SumX.Application.Employees.DTOs;

namespace SumX.Application.Employees.Queries.GetEmployeeById
{
    public sealed record GetEmployeeByIdQuery(Guid Id) : IQuery<EmployeeDto>;
}
