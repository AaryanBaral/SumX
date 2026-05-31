using SumX.Application.Common.CQRS;

namespace SumX.Application.Tenants.Commands.CreateTenant;

public sealed record CreateTenantCommand(
    string Name,
    string Email,
    string TenantId,
    string AdminPassword) : ICommand<Guid>;
