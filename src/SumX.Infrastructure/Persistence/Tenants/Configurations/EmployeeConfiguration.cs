using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SumX.Domain.Entities.Tenants;

namespace SumX.Infrastructure.Persistence.Tenants.Configurations
{
    public sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasMaxLength(36);

            builder.Property(e => e.FullName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Email)
                .IsRequired()
                .HasColumnName("EmailAddress")
                .HasMaxLength(256);
        }
    }
}
