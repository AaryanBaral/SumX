using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SumX.Domain.Entities.Master;

namespace SumX.Infrastructure.Persistence.Master.Configurations;

public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");

        builder.HasKey(tenant => tenant.Id);

        builder.Property(tenant => tenant.Id);

        builder.Property(tenant => tenant.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(tenant => tenant.Email)
            .IsRequired()
            .HasColumnName("EmailAddress")
            .HasMaxLength(256);

        builder.Property(tenant => tenant.TenantId)
            .IsRequired()
            .HasMaxLength(4)
            .IsFixedLength();

        builder.Property(tenant => tenant.DatabaseConnectionString)
            .IsRequired()
            .HasColumnName("DbConnStr")
            .HasMaxLength(2048);

        builder.HasIndex(tenant => tenant.TenantId)
            .IsUnique();
    }
}
