using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SumX.Infrastructure.Persistence.Master.Identity;

namespace SumX.Infrastructure.Persistence.Master.Seed
{
    public static class MasterDbSeeder
    {
        public static async Task<bool> SeedAsync(
            UserManager<MasterApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager)
        {
            // 1. Ensure Roles
            string[] roles = ["SuperAdmin", "Admin", "Employee"];

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }

            // 2. Check SuperAdmin
            var email = "assessment@yopmail.com";

            var user = await userManager.FindByEmailAsync(email);

            if (user != null)
            {
                return false;
            }

            user = new MasterApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                Role = "SuperAdmin",
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, "Tester@123");

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await userManager.AddToRoleAsync(user, "SuperAdmin");
            return true;
        }
    }
}