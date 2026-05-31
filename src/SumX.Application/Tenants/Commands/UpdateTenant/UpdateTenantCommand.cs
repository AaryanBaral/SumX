using SumX.Application.Common.Abstractions.CQRS;

namespace SumX.Application.Tenants.Commands.UpdateTenant;

public sealed record UpdateTenantCommand(
    Guid Id,
    string Name,
    string Email) : ICommand<Guid>;
