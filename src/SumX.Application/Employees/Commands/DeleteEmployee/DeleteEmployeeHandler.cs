using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SumX.Application.Common.Abstractions;
using SumX.Application.Common.Abstractions.Persistence;
using SumX.Application.Common.Exceptions;

namespace SumX.Application.Employees.Commands.DeleteEmployee
{
    public sealed class DeleteEmployeeHandler : IRequestHandler<DeleteEmployeeCommand, Guid>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICurrentUserContext _currentUserContext;

        public DeleteEmployeeHandler(IEmployeeRepository employeeRepository, ICurrentUserContext currentUserContext)
        {
            _employeeRepository = employeeRepository;
            _currentUserContext = currentUserContext;
        }

        public async Task<Guid> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            // Only Admin role can delete employees
            if (!string.Equals(_currentUserContext.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                throw new ForbiddenException("Only Admins can delete employees.");
            }

            var employee = await _employeeRepository.GetByIdAsync(request.Id, trackChanges: false);
            if (employee == null)
            {
                throw new NotFoundException($"Employee with ID '{request.Id}' was not found.");
            }

            await _employeeRepository.DeleteAsync(request.Id);

            return request.Id;
        }
    }
}
