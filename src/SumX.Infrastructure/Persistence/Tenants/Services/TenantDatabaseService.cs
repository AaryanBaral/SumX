using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SumX.Application.Common.Abstractions;

namespace SumX.Infrastructure.Persistence.Tenants.Services
{
    public class TenantDatabaseService : ITenantDatabaseService
    {
        private readonly DbContextOptions<TenantDbContext> _dbContextOptions;

        public TenantDatabaseService(DbContextOptions<TenantDbContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        public async Task CreateTenantDatabaseAsync(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            // Dynamically resolve tenant context for schema migrations using connectionString.
            // We use a dummy provider that returns the connection string since connection resolution in TenantDbContext depends on it.
            var dummyProvider = new DummyTenantProvider(connectionString);
            using var context = new TenantDbContext(optionsBuilder.Options, dummyProvider);
            
            // Applies database creation and runs any pending migrations
            await context.Database.MigrateAsync();
        }

        public async Task DeleteTenantDatabaseAsync(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            var dummyProvider = new DummyTenantProvider(connectionString);
            await using var context = new TenantDbContext(optionsBuilder.Options, dummyProvider);
            await context.Database.EnsureDeletedAsync();
        }

        private class DummyTenantProvider : ITenantProvider
        {
            private readonly string _connectionString;

            public DummyTenantProvider(string connectionString)
            {
                _connectionString = connectionString;
            }

            public Guid? TenantId => null;

            public Task SetTenantAsync(Guid tenantId)
            {
                return Task.CompletedTask;
            }

            public Task<string> GetConnectionStringAsync()
            {
                return Task.FromResult(_connectionString);
            }
        }
    }
}
