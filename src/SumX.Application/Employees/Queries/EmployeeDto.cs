using System;

namespace SumX.Application.Employees.Queries
{
    public sealed record EmployeeDto(
        Guid Id,
        string FullName,
        string Email);
}
