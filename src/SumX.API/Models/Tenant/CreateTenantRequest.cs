namespace SumX.API.Models.Tenant
{
    public sealed record CreateTenantRequest(
        string Name,
        string Email,
        string TenantId,
        string DatabaseConnectionString,
        string AdminPassword);
}
