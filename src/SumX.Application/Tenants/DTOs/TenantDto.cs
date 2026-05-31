namespace SumX.Application.Tenants.DTOs;

public sealed record TenantDto(
    Guid Id,
    string Name,
    string Email,
    string TenantId,
    string DatabaseConnectionString);
