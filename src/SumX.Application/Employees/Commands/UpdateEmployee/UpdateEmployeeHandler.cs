using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SumX.Application.Common.Abstractions;
using SumX.Application.Common.Abstractions.Persistence;
using SumX.Application.Common.Exceptions;

namespace SumX.Application.Employees.Commands.UpdateEmployee
{
    public sealed class UpdateEmployeeHandler : IRequestHandler<UpdateEmployeeCommand, Guid>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICurrentUserContext _currentUserContext;

        public UpdateEmployeeHandler(IEmployeeRepository employeeRepository, ICurrentUserContext currentUserContext)
        {
            _employeeRepository = employeeRepository;
            _currentUserContext = currentUserContext;
        }

        public async Task<Guid> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            // Only Admin role can update employees
            if (!string.Equals(_currentUserContext.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                throw new ForbiddenException("Only Admins can update employees.");
            }

            var employee = await _employeeRepository.GetByIdAsync(request.Id, trackChanges: true);
            if (employee == null)
            {
                throw new NotFoundException($"Employee with ID '{request.Id}' was not found.");
            }

            employee.Rename(request.FullName);
            employee.ChangeEmail(request.Email);

            await _employeeRepository.UpdateAsync(employee);

            return employee.Id;
        }
    }
}
