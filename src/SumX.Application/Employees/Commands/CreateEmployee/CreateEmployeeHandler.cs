using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SumX.Application.Common.Abstractions;
using SumX.Application.Common.Abstractions.Security;
using SumX.Application.Common.Abstractions.Persistence.Tenants;
using SumX.Application.Common.Exceptions;
using SumX.Domain.Entities.Tenants;

namespace SumX.Application.Employees.Commands.CreateEmployee
{
    public sealed class CreateEmployeeHandler : IRequestHandler<CreateEmployeeCommand, Guid>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICurrentUserContext _currentUserContext;

        public CreateEmployeeHandler(IEmployeeRepository employeeRepository, ICurrentUserContext currentUserContext)
        {
            _employeeRepository = employeeRepository;
            _currentUserContext = currentUserContext;
        }

        public async Task<Guid> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            if (!string.Equals(_currentUserContext.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                throw new ForbiddenException("Only Admins can create employees.");
            }

            var employeeId = Guid.NewGuid();
            var employee = Employee.Create(employeeId, request.FullName, request.Email);
            await _employeeRepository.CreateAsync(employee);
            return employeeId;
        }
    }
}
