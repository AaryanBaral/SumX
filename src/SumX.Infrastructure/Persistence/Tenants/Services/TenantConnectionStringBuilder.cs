using Microsoft.Extensions.Configuration;
using Npgsql;
using SumX.Application.Common.Abstractions;

namespace SumX.Infrastructure.Persistence.Tenants.Services;

public sealed class TenantConnectionStringBuilder : ITenantConnectionStringBuilder
{
    private readonly NpgsqlConnectionStringBuilder _masterBuilder;

    public TenantConnectionStringBuilder(IConfiguration configuration)
    {
        var masterConnectionString = configuration.GetConnectionString("MasterDb")
            ?? throw new InvalidOperationException("Connection string 'MasterDb' is not configured.");

        _masterBuilder = new NpgsqlConnectionStringBuilder(masterConnectionString);
    }

    public string GetDatabaseName(string tenantCode)
    {
        var normalizedCode = tenantCode.Trim().ToLowerInvariant();
        return $"sumx_tenant_{normalizedCode}";
    }

    public string Build(string tenantCode)
    {
        var builder = new NpgsqlConnectionStringBuilder(_masterBuilder.ConnectionString)
        {
            Database = GetDatabaseName(tenantCode)
        };

        return builder.ConnectionString;
    }
}
