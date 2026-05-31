using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SumX.Application.Auth;

namespace SumX.API.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddAPI(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtSettings>()
            .Bind(configuration.GetSection(JwtSettings.SectionName))
            .Configure(options =>
            {
                if (string.IsNullOrWhiteSpace(options.SecretKey))
                {
                    options.SecretKey = configuration["SUMX_JWT_SECRET"]
                        ?? Environment.GetEnvironmentVariable("SUMX_JWT_SECRET")
                        ?? string.Empty;
                }
            })
            .Validate(
                options => options.SecretKey.Length >= 32,
                "JWT secret must be at least 32 characters. Set Jwt:SecretKey or SUMX_JWT_SECRET.")
            .ValidateOnStart();

        services.AddJwtAuthentication(configuration);
        return services;
    }

    private static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwt = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
                    ?? throw new InvalidOperationException(
                        $"JWT configuration section '{JwtSettings.SectionName}' is missing.");

                var secretKey = jwt.SecretKey;
                if (string.IsNullOrWhiteSpace(secretKey))
                {
                    secretKey = configuration["SUMX_JWT_SECRET"]
                        ?? Environment.GetEnvironmentVariable("SUMX_JWT_SECRET")
                        ?? string.Empty;
                }

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secretKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        return services;
    }
}
