using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OnlineVehicleRentalSystem.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class DbInitializer
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string adminRole = "Admin";
        string adminEmail = "admin@gmail.com";

        // Generate a strong random password
        var passwordOptions = userManager.Options.Password;
        var randomPassword = GenerateRandomPassword(passwordOptions);

        // Ensure the Admin role exists
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole(adminRole));
            if (!roleResult.Succeeded)
            {
                Console.WriteLine($"Error creating role '{adminRole}':");
                foreach (var error in roleResult.Errors)
                {
                    Console.WriteLine($"- {error.Description}");
                }
                return;
            }
        }

        // Ensure the Admin user exists
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                Name = "Admin User"
            };

            var userResult = await userManager.CreateAsync(adminUser, randomPassword);
            if (userResult.Succeeded)
            {
                var roleAssignResult = await userManager.AddToRoleAsync(adminUser, adminRole);
                if (roleAssignResult.Succeeded)
                {
                    Console.WriteLine($"Admin user '{adminEmail}' created and assigned to role '{adminRole}'.");
                    Console.WriteLine($"Generated Admin Password: {randomPassword}");
                }
                else
                {
                    Console.WriteLine($"Error assigning role '{adminRole}' to user '{adminEmail}':");
                    foreach (var error in roleAssignResult.Errors)
                    {
                        Console.WriteLine($"- {error.Description}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Error creating admin user '{adminEmail}':");
                foreach (var error in userResult.Errors)
                {
                    Console.WriteLine($"- {error.Description}");
                }
            }
        }
        else
        {
            // Ensure the existing Admin user has the Admin role
            if (!await userManager.IsInRoleAsync(adminUser, adminRole))
            {
                var roleAssignResult = await userManager.AddToRoleAsync(adminUser, adminRole);
                if (roleAssignResult.Succeeded)
                {
                    Console.WriteLine($"Admin user '{adminEmail}' is already assigned to role '{adminRole}'.");
                }
                else
                {
                    Console.WriteLine($"Error assigning role '{adminRole}' to user '{adminEmail}':");
                    foreach (var error in roleAssignResult.Errors)
                    {
                        Console.WriteLine($"- {error.Description}");
                    }
                }
            }
            Console.WriteLine($"Admin user '{adminEmail}' already exists.");
        }
    }

    private static string GenerateRandomPassword(PasswordOptions opts)
    {
        var randomChars = new[]
        {
            "ABCDEFGHJKLMNPQRSTUVWXYZ",    // uppercase
            "abcdefghijkmnpqrstuvwxyz",    // lowercase
            "0123456789",                   // digits
            "!@$?_-"                        // non-alphanumeric
        };

        var rand = new Random(Environment.TickCount);
        var chars = new List<char>();

        chars.Insert(rand.Next(0, chars.Count),
            randomChars[0][rand.Next(0, randomChars[0].Length)]);
        chars.Insert(rand.Next(0, chars.Count),
            randomChars[1][rand.Next(0, randomChars[1].Length)]);
        chars.Insert(rand.Next(0, chars.Count),
            randomChars[2][rand.Next(0, randomChars[2].Length)]);
        chars.Insert(rand.Next(0, chars.Count),
            randomChars[3][rand.Next(0, randomChars[3].Length)]);

        for (int i = chars.Count; i < opts.RequiredLength
             || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
        {
            var rcs = randomChars[rand.Next(0, randomChars.Length)];
            chars.Insert(rand.Next(0, chars.Count),
                rcs[rand.Next(0, rcs.Length)]);
        }

        return new string(chars.ToArray());
    }
}
