using SimpleLogisticSystem.Data;
using SimpleLogisticSystem.Models;
using SimpleLogisticSystem.Data.Enum;
using Microsoft.AspNetCore.Identity;

namespace SimpleLogisticSystem
{
    public class Seed
    {
        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                //Roles
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                if (!await roleManager.RoleExistsAsync(UserRoles.User))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
                if (!await roleManager.RoleExistsAsync(UserRoles.InventoryManager))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.InventoryManager));

                //Admin
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                string adminUserEmail = "admin@admin.com";

                var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
                if (adminUser == null)
                {
                    var newAdminUser = new AppUser()
                    {
                        AccountType = UserRoles.Admin,
                        UserName = "AdminAdmin",
                        FirstName = "Admin",
                        LastName = "Admin",
                        Email = adminUserEmail,
                        EmailConfirmed = true,
                        Address = new Address()
                        {
                            Street = "adminStreet",
                            City = "adminCity",
                            PostalCode = "123 45",
                            Country = "adminCountry"
                        }
                    };
                    await userManager.CreateAsync(newAdminUser, "Admin@123");
                    await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
                }

                // Inventory Manager
                string inventoryManagerEmail = "invman@invman.com";

                var inventoryManagerUser = await userManager.FindByEmailAsync(inventoryManagerEmail);
                if (inventoryManagerUser == null)
                {
                    var newInventoryManagerUser = new AppUser()
                    {
                        AccountType = UserRoles.InventoryManager,
                        UserName = "InvManager",
                        FirstName = "Inventory",
                        LastName = "Manager",
                        Email = inventoryManagerEmail,
                        EmailConfirmed = true,
                        Address = new Address()
                        {
                            Street = "invStreet",
                            City = "invCity",
                            PostalCode = "123 45",
                            Country = "invCountry"
                        }
                    };
                    await userManager.CreateAsync(newInventoryManagerUser, "Invman@123");
                    await userManager.AddToRoleAsync(newInventoryManagerUser, UserRoles.InventoryManager);
                }
            }
        }
    }
}
