using FluentValidation;

namespace SumX.Application.Employees.Commands.DeleteEmployee
{
    public sealed class DeleteEmployeeValidator : AbstractValidator<DeleteEmployeeCommand>
    {
        public DeleteEmployeeValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Employee ID is required.");
        }
    }
}
