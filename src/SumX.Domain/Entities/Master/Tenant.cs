using SumX.Domain.Exceptions;

namespace SumX.Domain.Entities.Master;

public sealed class Tenant
{
    private Tenant(
        string id,
        string name,
        string email,
        string tenantId,
        string databaseConnectionString)
    {
        Id = ValidateId(id);
        Name = ValidateRequired(name, "Tenant name");
        Email = email ?? throw new ArgumentNullException(nameof(email));
        TenantId = ValidateTenantId(tenantId);
        DatabaseConnectionString = ValidateRequired(databaseConnectionString, "Tenant database connection string");
    }

    public string Id { get; }

    public string Name { get; private set; }

    public string Email { get; private set; }

    public string TenantId { get; }

    public string DatabaseConnectionString { get; private set; }

    public static Tenant Create(
        string id,
        string name,
        string email,
        string tenantId,
        string databaseConnectionString) =>
        new(id, name, email, tenantId, databaseConnectionString);

    public void Rename(string name)
    {
        Name = ValidateRequired(name, "Tenant name");
    }

    public void ChangeEmail(string email)
    {
        Email = email ?? throw new ArgumentNullException(nameof(email));
    }

    public void ChangeDatabaseConnectionString(string databaseConnectionString)
    {
        DatabaseConnectionString = ValidateRequired(databaseConnectionString, "Tenant database connection string");
    }

    public string GetDatabaseName() => TenantId;

    private static string ValidateId(string id) =>
        ValidateRequired(id, "Tenant id");

    private static string ValidateTenantId(string tenantId)
    {
        var normalizedTenantId = ValidateRequired(tenantId, "Tenant code").ToUpperInvariant();

        if (normalizedTenantId.Length != 4)
        {
            throw new DomainException("Tenant code must be exactly 4 characters.");
        }

        return normalizedTenantId;
    }

    private static string ValidateRequired(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"{fieldName} is required.");
        }

        return value.Trim();
    }
}
