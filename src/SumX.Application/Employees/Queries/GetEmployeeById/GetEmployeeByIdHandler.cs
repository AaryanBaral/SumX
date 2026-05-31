using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SumX.Application.Common.Abstractions;
using SumX.Application.Common.Abstractions.Security;
using SumX.Application.Common.Abstractions.Persistence.Tenants;
using SumX.Application.Common.Exceptions;
using SumX.Application.Employees.DTOs;
using SumX.Application.Employees.Mapping;

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

            if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(role, "Employee", StringComparison.OrdinalIgnoreCase))
            {
                throw new ForbiddenException("You are not authorized to view employee details.");
            }

            var employee = await _employeeRepository.GetByIdAsync(request.Id, trackChanges: false);
            if (employee == null)
            {
                throw new NotFoundException($"Employee with ID '{request.Id}' was not found.");
            }

            if (string.Equals(role, "Employee", StringComparison.OrdinalIgnoreCase) && 
                !string.Equals(employee.Email, _currentUserContext.Email, StringComparison.OrdinalIgnoreCase))
            {
                throw new ForbiddenException("Employees can only retrieve their own employee information.");
            }

            return employee.ToDto();
        }
    }
}
