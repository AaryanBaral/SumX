using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SumX.Domain.Entities.Master;
using SumX.Infrastructure.Persistence.Master.Identity;

namespace SumX.Infrastructure.Persistence.Master;

public sealed class MasterDbContext : IdentityDbContext<MasterApplicationUser, IdentityRole<Guid>, Guid>
{
    public MasterDbContext(DbContextOptions<MasterDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(MasterDbContext).Assembly,
            type => type.Namespace is not null &&
                    type.Namespace.Contains(".Persistence.Master.", StringComparison.Ordinal));
    }

    
}
