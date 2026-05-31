using FluentValidation;

namespace SumX.Application.Tenants.Queries.GetTenantById;

public sealed class GetTenantByIdValidator : AbstractValidator<GetTenantByIdQuery>
{
    public GetTenantByIdValidator()
    {
        RuleFor(q => q.Id)
            .NotEmpty().WithMessage("Tenant id is required.");
    }
}
