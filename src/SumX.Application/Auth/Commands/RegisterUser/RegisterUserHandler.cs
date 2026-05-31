using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Results;
using MediatR;
using SumX.Application.Common.Abstractions;
using SumX.Application.Common.Abstractions.Security;
using SumX.Application.Common.Abstractions.Persistence.Tenants;
using SumX.Application.Common.Exceptions;
using SumX.Domain.Constants;
using SumX.Domain.Entities;
using SumX.Domain.Entities.Tenants;

namespace SumX.Application.Auth.Commands.RegisterUser
{
    public sealed class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICurrentUserContext _currentUserContext;

        public RegisterUserHandler(
            IUserRepository userRepository,
            IEmployeeRepository employeeRepository,
            ICurrentUserContext currentUserContext)
        {
            _userRepository = userRepository;
            _employeeRepository = employeeRepository;
            _currentUserContext = currentUserContext;
        }

        public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            if (!string.Equals(_currentUserContext.Role, Roles.Admin, StringComparison.Ordinal))
            {
                throw new ForbiddenException("Only Admins can register new users.");
            }

            var adminTenantId = _currentUserContext.TenantId;
            if (!adminTenantId.HasValue)
            {
                throw new ForbiddenException("Admin user must belong to a tenant context.");
            }

            var userExists = await _userRepository.ExistsByEmailAsync(request.Email);
            if (userExists)
            {
                throw new AppValidationException(
                    new[]
                    {
                        new ValidationFailure(
                            nameof(RegisterUserCommand.Email),
                            $"Email address '{request.Email}' is already registered.")
                    });
            }

            var newUser = ApplicationUser.CreateTenantUser(
                id: Guid.NewGuid(),
                emailAddress: request.Email,
                tenantId: adminTenantId.Value,
                role: request.Role);

            var userId = await _userRepository.CreateAsync(newUser, request.Password);

            await _userRepository.AssignRoleAsync(userId, request.Role);

            if (string.Equals(request.Role, Roles.Employee, StringComparison.Ordinal))
            {
                var existingEmployee = await _employeeRepository.GetByEmailAsync(request.Email);
                if (existingEmployee is null)
                {
                    var displayName = request.Email.Split('@')[0];
                    var employee = Employee.Create(Guid.NewGuid(), displayName, request.Email);
                    await _employeeRepository.CreateAsync(employee);
                }
            }

            return userId;
        }
    }
}
