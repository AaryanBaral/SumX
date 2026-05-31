using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SumX.Application;

namespace SumX.Infrastructure.DependencyInjection
{
    public static class DependencyInjectionExtensions
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
}
