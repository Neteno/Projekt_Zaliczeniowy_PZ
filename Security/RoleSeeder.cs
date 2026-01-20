using Microsoft.AspNetCore.Identity;
using Projekt_Zaliczeniowy_PZ.Models;

namespace Projekt_Zaliczeniowy_PZ.Security
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(
            RoleManager<IdentityRole> roleManager,
            UserManager<AppUser> userManager)
        {
            // App Role generator
            if (!await roleManager.RoleExistsAsync(AppRoles.Admin))
                await roleManager.CreateAsync(new IdentityRole(AppRoles.Admin));

            if (!await roleManager.RoleExistsAsync(AppRoles.User))
                await roleManager.CreateAsync(new IdentityRole(AppRoles.User));

            // Users check
            var users = userManager.Users.ToList();
            if (users.Count == 0)
                return;

            // Admin creation
            var firstUser = users.First();

            var firstUserRoles = await userManager.GetRolesAsync(firstUser);
            if (!firstUserRoles.Contains(AppRoles.Admin))
                await userManager.AddToRoleAsync(firstUser, AppRoles.Admin);

            if (!firstUserRoles.Contains(AppRoles.User))
                await userManager.AddToRoleAsync(firstUser, AppRoles.User);

            // Users skipping
            foreach (var u in users.Skip(1))
            {
                var roles = await userManager.GetRolesAsync(u);
                if (roles == null || roles.Count == 0)
                {
                    await userManager.AddToRoleAsync(u, AppRoles.User);
                }
            }
        }
    }
}
