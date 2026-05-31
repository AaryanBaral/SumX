using System;

namespace SumX.Application.Employees.DTOs
{
    public sealed record EmployeeDto(
        Guid Id,
        string FullName,
        string Email);
}
