using FluentValidation;

namespace SumX.Application.Tenants.Commands.CreateTenant
{
    public sealed class CreateTenantValidator : AbstractValidator<CreateTenantCommand>
    {
        public CreateTenantValidator()
        {
            RuleFor(v => v.Name)
                .NotEmpty().WithMessage("Tenant name is required.")
                .MaximumLength(200);

            RuleFor(v => v.Email)
                .NotEmpty().WithMessage("Tenant admin email is required.")
                .EmailAddress().WithMessage("Must be a valid email address.")
                .MaximumLength(256);

            RuleFor(v => v.TenantId)
                .NotEmpty().WithMessage("Tenant code is required.")
                .Length(4).WithMessage("Tenant code must be exactly 4 characters.");

            RuleFor(v => v.AdminPassword)
                .NotEmpty().WithMessage("Admin password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.");
        }
    }
}
