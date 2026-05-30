using FluentValidation;

namespace SumX.Application.Employees.Commands.UpdateEmployee
{
    public sealed class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeCommand>
    {
        public UpdateEmployeeValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Employee ID is required.");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Employee full name is required.")
                .MaximumLength(200).WithMessage("Full name must not exceed 200 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Employee email address is required.")
                .EmailAddress().WithMessage("Email address is invalid.")
                .MaximumLength(256).WithMessage("Email must not exceed 256 characters.");
        }
    }
}
