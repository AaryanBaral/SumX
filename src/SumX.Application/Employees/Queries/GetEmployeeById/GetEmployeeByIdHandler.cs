using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SumX.Application.Common.Abstractions;
using SumX.Application.Common.Abstractions.Persistence;
using SumX.Application.Common.Exceptions;

namespace SumX.Application.Employees.Queries.GetEmployeeById
{
    public sealed class GetEmployeeByIdHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeDto>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICurrentUserContext _currentUserContext;

        public GetEmployeeByIdHandler(IEmployeeRepository employeeRepository, ICurrentUserContext currentUserContext)
        {
            _employeeRepository = employeeRepository;
            _currentUserContext = currentUserContext;
        }

        public async Task<EmployeeDto> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            var role = _currentUserContext.Role;

            // Enforce role checks: Admin or Employee only
            if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(role, "Employee", StringComparison.OrdinalIgnoreCase))
            {
                throw new ForbiddenException("You are not authorized to view employee details.");
            }

            // If user is an Employee, they can only retrieve their own record (Id must match UserId)
            if (string.Equals(role, "Employee", StringComparison.OrdinalIgnoreCase) && request.Id != _currentUserContext.UserId)
            {
                throw new ForbiddenException("Employees can only retrieve their own employee information.");
            }

            var employee = await _employeeRepository.GetByIdAsync(request.Id, trackChanges: false);
            if (employee == null)
            {
                throw new NotFoundException($"Employee with ID '{request.Id}' was not found.");
            }

            return new EmployeeDto(employee.Id, employee.FullName, employee.Email);
        }
    }
}
