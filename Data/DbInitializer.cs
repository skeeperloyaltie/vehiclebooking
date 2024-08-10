using Microsoft.AspNetCore.Identity;
using OnlineVehicleRentalSystem.Models;

public static class DbInitializer
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string adminRole = "Admin";
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        string adminEmail = "admin@gmail.com";
        string adminPassword = "admin123";

        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new User { UserName = adminEmail, Email = adminEmail, Name = "Admin User" };
            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }
    }
}
