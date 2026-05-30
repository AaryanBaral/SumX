
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SumX.API.Extensions;
using SumX.API.Middlewares;
using SumX.Domain.Constants;
using SumX.Infrastructure;
using SumX.Infrastructure.Persistence.Master;
using SumX.Infrastructure.Persistence.Master.Identity;
using SumX.Infrastructure.Persistence.Master.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddInfrastructure(builder.Configuration);
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
builder.Services.AddSwaggerGen();

var app = builder.Build();


var cliArgs = Environment.GetCommandLineArgs();

if (cliArgs.Contains("seed"))
{
    using var scope = app.Services.CreateScope();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<MasterApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

    var seeded = await MasterDbSeeder.SeedAsync(userManager, roleManager);

    if (seeded)
    {
        Console.WriteLine("✅ Seeding completed");
    }
    else
    {
        Console.WriteLine("Already seeded");
    }
    return;
}


using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider
        .GetRequiredService<ILogger<Program>>();

    try
    {
        var conn = builder.Configuration.GetConnectionString("DefaultConnection");

        Console.WriteLine($"DB CONNECTION => {conn}");
        var dbContext = scope.ServiceProvider
            .GetRequiredService<MasterDbContext>();
        await dbContext.Database.OpenConnectionAsync();
        await dbContext.Database.CloseConnectionAsync();
        Console.WriteLine("✅ DB RAW CONNECTION SUCCESS");
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ ERROR WHILE CONNECTING DB:");
        Console.WriteLine(ex.ToString());

        logger.LogError(ex, "❌ Error while connecting to database.");
        logger.LogError(ex, "❌ Error while connecting to database.");
    }
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseMiddleware<TenantResolutionMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();
