namespace SumX.API.Models.Tenant
{
    public sealed record UpdateTenantRequest(
        string Name,
        string Email,
        string DatabaseConnectionString);
}
