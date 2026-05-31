namespace SumX.Application.Common.Abstractions;

public interface ITenantConnectionStringBuilder
{
    string GetDatabaseName(string tenantCode);

    string Build(string tenantCode);
}
