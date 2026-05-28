using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SumX.Domain.Constants;
using SumX.Domain.Entities.Master;
using SumX.Infrastructure.Persistence.Master.Identity;

namespace SumX.Infrastructure.Persistence.Master.Configurations;

public sealed class MasterApplicationUserConfiguration : IEntityTypeConfiguration<MasterApplicationUser>
{
    public void Configure(EntityTypeBuilder<MasterApplicationUser> builder)
    {
        builder.ToTable("Users", table =>
            table.HasCheckConstraint(
                "CK_Users_Role",
                $"\"Role\" IN ('{Roles.SuperAdmin}', '{Roles.Admin}', '{Roles.Employee}')"));

        builder.Property(user => user.TenantId)
            .HasMaxLength(36);

        builder.Property(user => user.Role)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(user => user.CreatedAt)
            .IsRequired();

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(user => user.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
