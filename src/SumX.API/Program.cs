
using SumX.API.Extensions;
using SumX.API.Middlewares;
using SumX.Application.Common.Abstractions;
using SumX.Application.Common.Constants;
using SumX.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddSumX(builder.Configuration);
builder.Services.AddAPI(builder.Configuration);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireAuthenticatedUser().RequireRole(Roles.Admin));
});
builder.Services.AddControllers();
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithJwt();

var app = builder.Build();

var cliArgs = Environment.GetCommandLineArgs();

if (cliArgs.Contains("seed"))
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<IMasterDbSeeder>();
    var seeded = await seeder.SeedAsync();

    Console.WriteLine(seeded ? "✅ Seeding completed" : "Already seeded");
    return;
}

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        var migrator = scope.ServiceProvider.GetRequiredService<IMasterDatabaseMigrator>();
        await migrator.MigrateAsync();
        logger.LogInformation("Master database migrations applied successfully.");

        var seeder = scope.ServiceProvider.GetRequiredService<IMasterDbSeeder>();
        var seeded = await seeder.SeedAsync();
        if (seeded)
        {
            logger.LogInformation("Master database seeded successfully.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to apply master database migrations/seeding.");
        throw;
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", async () =>
{
    return "The api is working";
});
app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<TransactionMiddleware>();
app.UseAuthentication();
app.UseMiddleware<TenantResolutionMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();
