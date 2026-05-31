using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SumX.Infrastructure.Persistence.Tenants
{
    /// <summary>
    /// Design-time factory for TenantDbContext.
    /// EF Core CLI tools (dotnet ef migrations add, dotnet ef database update)
    /// use this factory when there is no running host (i.e., no HttpContext / ITenantProvider).
    /// It reads the "TenantDesignTime" connection string from appsettings.json in the API project.
    /// </summary>
    public sealed class TenantDbContextDesignTimeFactory : IDesignTimeDbContextFactory<TenantDbContext>
    {
        public TenantDbContext CreateDbContext(string[] args)
        {
            // Build configuration by pointing to the API project where appsettings.json lives.
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../SumX.API"))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
                .Build();

            var connectionString = configuration.GetConnectionString("TenantDesignTime")
                ?? "Host=127.0.0.1;Port=5432;Database=sumx_tenant_design;Username=postgres;Password=postgres";

            var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            // Pass null for ITenantProvider — it is not needed at design-time.
            return new TenantDbContext(optionsBuilder.Options, tenantProvider: null!);
        }
    }
}
