using Identity.Domain.Entity;
using Identity.Domain.Enums;
using Identity.Presentation.Constants;
using Microsoft.AspNetCore.Identity;

namespace Identity.Presentation.Initializer
{
    public class DataDbInitializer
    {
        public static async Task Initialize(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await EnsureRoleExistsAsync(roleManager, Roles.Admin.ToString());
            await EnsureRoleExistsAsync(roleManager, Roles.User.ToString());
            await EnsureRoleExistsAsync(roleManager, Roles.Manager.ToString());
            await EnsureAdminUserExistsAsync(userManager);
        }

        private static async Task EnsureRoleExistsAsync(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole(roleName));
        }

        private static async Task EnsureAdminUserExistsAsync(UserManager<ApplicationUser> userManager)
        {
            var adminUser = await userManager.FindByEmailAsync(SeedDataConstants.AdminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = SeedDataConstants.AdminEmail,
                    Email = SeedDataConstants.AdminEmail,
                    EmailConfirmed = true,
                    Status = Statuses.Activity,
                    FirstName = SeedDataConstants.AdminFirstName,
                    LastName = SeedDataConstants.AdminLastName,
                };

                var result = await userManager.CreateAsync(adminUser, SeedDataConstants.AdminPassword);

                if (result.Succeeded)
                    await userManager.AddToRoleAsync(adminUser, Roles.Admin.ToString());
            }
        }
    }
}
