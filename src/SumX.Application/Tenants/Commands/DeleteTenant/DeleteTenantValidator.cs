using FluentValidation;

namespace SumX.Application.Tenants.Commands.DeleteTenant;

public sealed class DeleteTenantValidator : AbstractValidator<DeleteTenantCommand>
{
    public DeleteTenantValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Tenant id is required.");
    }
}
