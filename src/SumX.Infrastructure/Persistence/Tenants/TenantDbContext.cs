using System;
using Microsoft.EntityFrameworkCore;
using SumX.Application.Common.Abstractions;
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
                // Retrieve the connection string dynamically from the tenant provider
                var connectionString = _tenantProvider.GetConnectionStringAsync().GetAwaiter().GetResult();
                optionsBuilder.UseNpgsql(connectionString);
            }
            
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Employee>(builder =>
            {
                builder.ToTable("Employees");
                builder.HasKey(e => e.Id);
                builder.Property(e => e.Id).HasMaxLength(36);
                builder.Property(e => e.FullName).IsRequired().HasMaxLength(200);
                builder.Property(e => e.Email).IsRequired().HasMaxLength(256);
            });
        }
    }
}