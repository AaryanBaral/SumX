using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SumX.Application.Auth;
using SumX.Application.Common.Abstractions;
using SumX.Application.Common.Abstractions.MultiTenancy;
using SumX.Application.Common.Abstractions.Security;
using SumX.Application.Common.Abstractions.Persistence.Tenants;
using SumX.Application.Common.Abstractions.Persistence.Master;
using SumX.Infrastructure.Auth;
using SumX.Infrastructure.Persistence.Master;
using SumX.Infrastructure.Persistence.Master.Identity;
using SumX.Infrastructure.Persistence.Master.Repositories;
using SumX.Infrastructure.Persistence.Master.Seed;
using SumX.Infrastructure.Persistence.Tenants;
using SumX.Infrastructure.Persistence.Tenants.Repositories;

namespace SumX.Infrastructure.DependencyInjection;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var masterConnectionString = configuration.GetConnectionString("MasterDb")
            ?? throw new InvalidOperationException("Connection string 'MasterDb' is not configured.");

        services.AddDbContext<MasterDbContext>(options =>
            options.UseNpgsql(
                masterConnectionString,
                npgsql => npgsql.MigrationsAssembly(typeof(MasterDbContext).Assembly.FullName)));

        services.AddIdentityCore<MasterApplicationUser>(options =>
        {
            options.Password.RequiredLength = 6;
            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<MasterDbContext>();
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IMasterDbSeeder, MasterDbSeederService>();
        services.AddScoped<IMasterDatabaseMigrator, MasterDatabaseMigrator>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        
        services.AddScoped<ICurrentUserContext, CurrentUserContext>();
        services.AddScoped<ITenantProvider, TenantProvider>();
        services.AddSingleton<ITenantConnectionStringBuilder, Persistence.Tenants.Services.TenantConnectionStringBuilder>();
        services.AddScoped<ITenantDatabaseService, Persistence.Tenants.Services.TenantDatabaseService>();
        services.AddDbContext<TenantDbContext>();

        return services;
    }
}
