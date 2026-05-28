using FluentValidation;

namespace SumX.Application.User.Command.DeleteUser;

public sealed class DeleteUserValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
