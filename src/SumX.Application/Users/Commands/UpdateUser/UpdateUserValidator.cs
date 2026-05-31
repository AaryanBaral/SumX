using FluentValidation;
using SumX.Domain.Constants;

namespace SumX.Application.Users.Commands.UpdateUser;

public sealed class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(role => role == Roles.Admin || role == Roles.Employee)
            .WithMessage("Role must be either Admin or Employee.");
    }
}
