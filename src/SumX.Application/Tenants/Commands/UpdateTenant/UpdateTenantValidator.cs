using FluentValidation;

namespace SumX.Application.Tenants.Commands.UpdateTenant;

public sealed class UpdateTenantValidator : AbstractValidator<UpdateTenantCommand>
{
    public UpdateTenantValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Tenant id is required.");

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Tenant name is required.")
            .MaximumLength(200);

        RuleFor(v => v.Email)
            .NotEmpty().WithMessage("Tenant admin email is required.")
            .EmailAddress().WithMessage("Must be a valid email address.")
            .MaximumLength(256);
    }
}
