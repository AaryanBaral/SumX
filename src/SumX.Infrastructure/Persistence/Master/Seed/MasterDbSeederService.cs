using Microsoft.AspNetCore.Identity;
using SumX.Application.Common.Abstractions;
using SumX.Domain.Constants;
using SumX.Infrastructure.Persistence.Master.Identity;

namespace SumX.Infrastructure.Persistence.Master.Seed;

public sealed class MasterDbSeederService : IMasterDbSeeder
{
    private readonly UserManager<MasterApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public MasterDbSeederService(
        UserManager<MasterApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<bool> SeedAsync(CancellationToken cancellationToken = default)
    {
        foreach (var role in Roles.AllRoles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }

        const string email = "assessment@yopmail.com";

        if (await _userManager.FindByEmailAsync(email) is not null)
        {
            return false;
        }

        var user = new MasterApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            Role = Roles.SuperAdmin,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, "Tester@123");

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        await _userManager.AddToRoleAsync(user, Roles.SuperAdmin);
        return true;
    }
}
