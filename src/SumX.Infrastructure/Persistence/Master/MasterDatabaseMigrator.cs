using Microsoft.EntityFrameworkCore;
using SumX.Application.Common.Abstractions;

namespace SumX.Infrastructure.Persistence.Master;

public sealed class MasterDatabaseMigrator : IMasterDatabaseMigrator
{
    private readonly MasterDbContext _dbContext;

    public MasterDatabaseMigrator(MasterDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task MigrateAsync(CancellationToken cancellationToken = default) =>
        _dbContext.Database.MigrateAsync(cancellationToken);
}
