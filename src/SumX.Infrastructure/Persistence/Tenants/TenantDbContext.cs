using System;
using Microsoft.EntityFrameworkCore;
using SumX.Application.Common.Abstractions;
using SumX.Application.Common.Abstractions.MultiTenancy;
using SumX.Domain.Entities.Tenants;

namespace SumX.Infrastructure.Persistence.Tenants
{
    public class TenantDbContext : DbContext
    {
        private readonly ITenantProvider _tenantProvider;

        public TenantDbContext(
            DbContextOptions<TenantDbContext> options,
            ITenantProvider tenantProvider) : base(options)
        {
            _tenantProvider = tenantProvider;
        }

        public DbSet<Employee> Employees => Set<Employee>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString;
                try
                {
                    // Retrieve the connection string dynamically from the tenant provider
                    connectionString = _tenantProvider.GetConnectionStringAsync().GetAwaiter().GetResult();
                }
                catch (InvalidOperationException)
                {
                    // Fallback connection string for EF Core CLI design-time migration generation
                    connectionString = "Host=127.0.0.1;Port=5432;Database=sumx_tenant_design;Username=postgres;Password=postgres";
                }
                optionsBuilder.UseNpgsql(connectionString);
            }
            
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(TenantDbContext).Assembly,
                type => type.Namespace is not null &&
                        type.Namespace.Contains(".Persistence.Tenants.", StringComparison.Ordinal));
        }
    }
}