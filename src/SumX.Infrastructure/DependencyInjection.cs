using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SumX.Application.Auth.Interfaces;
using SumX.Application.Common.Abstractions;
using SumX.Application.Common.Abstractions.Persistence;
using SumX.Application.Common.Abstractions.Persistence.Master;
using SumX.Application.User.Interface;
using SumX.Infrastructure.Auth;
using SumX.Infrastructure.Persistence.Master;
using SumX.Infrastructure.Persistence.Master.Identity;
using SumX.Infrastructure.Persistence.Master.Repositories;
using SumX.Infrastructure.Persistence.Tenants;
using SumX.Infrastructure.Persistence.Tenants.Repositories;

namespace SumX.Infrastructure;

public static class DependencyInjection
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
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        
        services.AddScoped<ICurrentUserContext, CurrentUserContext>();
        services.AddScoped<ITenantProvider, TenantProvider>();
        services.AddScoped<ITenantDatabaseService, SumX.Infrastructure.Persistence.Tenants.Services.TenantDatabaseService>();
        services.AddDbContext<TenantDbContext>();

        return services;
    }
}
