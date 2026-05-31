namespace SumX.Application.Common.Abstractions.MultiTenancy;

public interface ITenantConnectionStringBuilder
{
    string GetDatabaseName(string tenantCode);

    string Build(string tenantCode);
}
