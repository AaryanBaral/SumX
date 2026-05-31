using System.Collections.Generic;
using SumX.Application.Common.Abstractions.CQRS;
using SumX.Application.Employees.DTOs;

namespace SumX.Application.Employees.Queries.GetAllEmployees
{
    public sealed record GetAllEmployeesQuery : IQuery<IEnumerable<EmployeeDto>>;
}
