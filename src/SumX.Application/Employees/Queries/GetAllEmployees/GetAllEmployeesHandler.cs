using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SumX.Application.Common.Abstractions;
using SumX.Application.Common.Abstractions.Persistence;
using SumX.Application.Common.Exceptions;

namespace SumX.Application.Employees.Queries.GetAllEmployees
{
    public sealed class GetAllEmployeesHandler : IRequestHandler<GetAllEmployeesQuery, IEnumerable<EmployeeDto>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICurrentUserContext _currentUserContext;

        public GetAllEmployeesHandler(IEmployeeRepository employeeRepository, ICurrentUserContext currentUserContext)
        {
            _employeeRepository = employeeRepository;
            _currentUserContext = currentUserContext;
        }

        public async Task<IEnumerable<EmployeeDto>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
        {
            // Only Admin role can view all employees
            if (!string.Equals(_currentUserContext.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                throw new ForbiddenException("Only Admins can list all employees.");
            }

            var employees = await _employeeRepository.GetAllAsync(trackChanges: false);

            return employees.Select(e => new EmployeeDto(e.Id, e.FullName, e.Email));
        }
    }
}
