using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SumX.Application.Auth.Commands.LoginUser;
using SumX.Application.Common.Behaviours;

namespace SumX.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(LoginUserCommand).Assembly));

        services.AddValidatorsFromAssembly(typeof(LoginUserCommand).Assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
