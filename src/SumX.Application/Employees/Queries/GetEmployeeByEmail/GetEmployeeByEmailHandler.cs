using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SumX.Application.Common.Abstractions;
using SumX.Application.Common.Abstractions.Persistence;
using SumX.Application.Common.Exceptions;

namespace SumX.Application.Employees.Queries.GetEmployeeByEmail
{
    public sealed class GetEmployeeByEmailHandler : IRequestHandler<GetEmployeeByEmailQuery, EmployeeDto>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICurrentUserContext _currentUserContext;

        public GetEmployeeByEmailHandler(IEmployeeRepository employeeRepository, ICurrentUserContext currentUserContext)
        {
            _employeeRepository = employeeRepository;
            _currentUserContext = currentUserContext;
        }

        public async Task<EmployeeDto> Handle(GetEmployeeByEmailQuery request, CancellationToken cancellationToken)
        {
            var role = _currentUserContext.Role;

            // Enforce role checks: Admin or Employee only
            if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(role, "Employee", StringComparison.OrdinalIgnoreCase))
            {
                throw new ForbiddenException("You are not authorized to view employee details.");
            }

            // If user is an Employee, they can only retrieve their own record (emails must match)
            if (string.Equals(role, "Employee", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(request.Email, _currentUserContext.Email, StringComparison.OrdinalIgnoreCase))
            {
                throw new ForbiddenException("Employees can only retrieve their own employee information.");
            }

            var employee = await _employeeRepository.GetByEmailAsync(request.Email, trackChanges: false);
            if (employee == null)
            {
                throw new NotFoundException($"Employee with email '{request.Email}' was not found.");
            }

            return new EmployeeDto(employee.Id, employee.FullName, employee.Email);
        }
    }
}
