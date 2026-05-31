using SumX.Application.Common.Abstractions.CQRS;

namespace SumX.Application.Tenants.Commands.DeleteTenant
{
    public sealed record DeleteTenantCommand(Guid Id) : ICommand<Guid>;
}
