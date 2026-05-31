using Microsoft.EntityFrameworkCore;
using Npgsql;
using SumX.Application.Common.Abstractions;
using SumX.Application.Common.Abstractions.MultiTenancy;

namespace SumX.Infrastructure.Persistence.Tenants.Services;

public sealed class TenantDatabaseService : ITenantDatabaseService
{
    public async Task CreateTenantDatabaseAsync(string connectionString, CancellationToken cancellationToken = default)
    {
        await EnsurePostgreSqlDatabaseExistsAsync(connectionString, cancellationToken);

        var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        var dummyProvider = new DummyTenantProvider(connectionString);
        await using var context = new TenantDbContext(optionsBuilder.Options, dummyProvider);
        await context.Database.MigrateAsync(cancellationToken);
    }

    public async Task DeleteTenantDatabaseAsync(string connectionString, CancellationToken cancellationToken = default)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        var dummyProvider = new DummyTenantProvider(connectionString);
        await using var context = new TenantDbContext(optionsBuilder.Options, dummyProvider);
        await context.Database.EnsureDeletedAsync(cancellationToken);
    }

    private static async Task EnsurePostgreSqlDatabaseExistsAsync(
        string connectionString,
        CancellationToken cancellationToken)
    {
        var tenantBuilder = new NpgsqlConnectionStringBuilder(connectionString);
        var databaseName = tenantBuilder.Database
            ?? throw new InvalidOperationException("Tenant connection string must include a database name.");

        var adminBuilder = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Database = "postgres"
        };

        await using var connection = new NpgsqlConnection(adminBuilder.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        await using var existsCommand = connection.CreateCommand();
        existsCommand.CommandText = "SELECT 1 FROM pg_database WHERE datname = @name";
        existsCommand.Parameters.AddWithValue("name", databaseName);

        var exists = await existsCommand.ExecuteScalarAsync(cancellationToken) is not null;
        if (exists)
        {
            return;
        }

        await using var createCommand = connection.CreateCommand();
        createCommand.CommandText = $"CREATE DATABASE \"{databaseName.Replace("\"", "\"\"")}\"";
        await createCommand.ExecuteNonQueryAsync(cancellationToken);
    }

    private sealed class DummyTenantProvider : ITenantProvider
    {
        private readonly string _connectionString;

        public DummyTenantProvider(string connectionString) => _connectionString = connectionString;

        public Guid? TenantId => null;

        public Task SetTenantAsync(Guid tenantId) => Task.CompletedTask;

        public Task<string> GetConnectionStringAsync() => Task.FromResult(_connectionString);
    }
}
