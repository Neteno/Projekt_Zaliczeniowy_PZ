using Microsoft.AspNetCore.Identity;
using Projekt_Zaliczeniowy_PZ.Models;

namespace Projekt_Zaliczeniowy_PZ.Security
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            if (!await roleManager.RoleExistsAsync(AppRoles.Admin))
                await roleManager.CreateAsync(new IdentityRole(AppRoles.Admin));

            if (!await roleManager.RoleExistsAsync(AppRoles.User))
                await roleManager.CreateAsync(new IdentityRole(AppRoles.User));

            // Każde konto bez roli dostaje domyślnie "User"
            foreach (var u in userManager.Users)
            {
                var roles = await userManager.GetRolesAsync(u);
                if (roles == null || roles.Count == 0)
                    await userManager.AddToRoleAsync(u, AppRoles.User);
            }
        }
    }
}
