using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SumX.Application;

namespace SumX.Infrastructure;

public static class SumXCompositionExtensions
{
    public static IServiceCollection AddSumX(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddApplication();
        services.AddInfrastructure(configuration);
        return services;
    }
}
