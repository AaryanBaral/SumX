using System.Collections.Generic;
using SumX.Application.Common.CQRS;

namespace SumX.Application.Employees.Queries.GetAllEmployees
{
    public sealed record GetAllEmployeesQuery : IQuery<IEnumerable<EmployeeDto>>;
}
