namespace SumX.Application.Common.Abstractions;

public interface IMasterDatabaseMigrator
{
    Task MigrateAsync(CancellationToken cancellationToken = default);
}
