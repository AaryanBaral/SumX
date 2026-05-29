using SumX.Application.Common.CQRS;

namespace SumX.Application.Tenants.Commands.UpdateTenant
{
    public sealed record UpdateTenantCommand(
        Guid Id,
        string Name,
        string Email,
        string DatabaseConnectionString) : ICommand<Guid>;
}
