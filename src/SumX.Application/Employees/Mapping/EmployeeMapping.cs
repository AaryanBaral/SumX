using SumX.Application.Employees.DTOs;
using SumX.Domain.Entities.Tenants;

namespace SumX.Application.Employees.Mapping
{
    internal static class EmployeeMapping
    {
        public static EmployeeDto ToDto(this Employee employee) =>
            new(employee.Id, employee.FullName, employee.Email);
    }
}
