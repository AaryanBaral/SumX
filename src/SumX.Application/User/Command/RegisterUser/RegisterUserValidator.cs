using FluentValidation;
using SumX.Application.User.Command.RegisterUser;
using SumX.Domain.Constants;

namespace SumX.Application.User.Command.RegisterUser;

public sealed class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one non-alphanumeric character.");

        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(role => role == Roles.Admin || role == Roles.Employee)
            .WithMessage("Role must be either Admin or Employee.");
    }
}
