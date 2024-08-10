using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineVehicleRentalSystem.Data;
using OnlineVehicleRentalSystem.Models;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Add Identity services, including support for roles
builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // Add this line to support roles
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Add logging services
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Ensure authentication middleware is added
app.UseAuthorization();

app.MapRazorPages(); // Ensure Razor Pages are mapped

// Map the ProfileController routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map a specific route for the ProfileController
app.MapControllerRoute(
    name: "profile",
    pattern: "Profile/{action=Index}/{id?}",
    defaults: new { controller = "Profile", action = "Index" }
);

// Seed Roles and Admin User
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var configuration = services.GetRequiredService<IConfiguration>();

    await SeedRolesAndAdminUser(userManager, roleManager, logger, configuration);
}

app.Run();

async Task SeedRolesAndAdminUser(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ILogger logger, IConfiguration configuration)
{
    string adminRole = "Admin";
    logger.LogInformation("Starting role and admin user seeding...");

    // Create the Admin role if it doesn't exist
    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        var roleResult = await roleManager.CreateAsync(new IdentityRole(adminRole));
        if (roleResult.Succeeded)
        {
            logger.LogInformation($"Role '{adminRole}' created successfully.");
        }
        else
        {
            logger.LogError($"Failed to create role '{adminRole}'.");
        }
    }
    else
    {
        logger.LogInformation($"Role '{adminRole}' already exists.");
    }

    var adminEmail = configuration.GetValue<string>("AdminUser:Email");
    var adminPassword = configuration.GetValue<string>("AdminUser:Password");

    // Check if the password is set in the configuration
    if (string.IsNullOrEmpty(adminPassword))
    {
        logger.LogError("Admin password is null or empty. Ensure it is set in the configuration.");
        return;
    }

    // Find or create the admin user
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new User { UserName = adminEmail, Email = adminEmail, Name = "Admin User" };

        var userResult = await userManager.CreateAsync(adminUser, adminPassword);
        if (userResult.Succeeded)
        {
            var roleAssignResult = await userManager.AddToRoleAsync(adminUser, adminRole);
            if (roleAssignResult.Succeeded)
            {
                logger.LogInformation($"Admin user '{adminEmail}' created and assigned to role '{adminRole}'.");
            }
            else
            {
                logger.LogError($"Failed to assign admin user '{adminEmail}' to role '{adminRole}'.");
            }
        }
        else
        {
            logger.LogError($"Failed to create admin user '{adminEmail}'. Errors:");
            foreach (var error in userResult.Errors)
            {
                logger.LogError($"- {error.Description}");
            }
        }
    }
    else
    {
        logger.LogInformation($"Admin user '{adminEmail}' already exists.");

        // If the user exists, you can update the password here
        var token = await userManager.GeneratePasswordResetTokenAsync(adminUser);
        var resetResult = await userManager.ResetPasswordAsync(adminUser, token, adminPassword);
        if (resetResult.Succeeded)
        {
            logger.LogInformation($"Admin user '{adminEmail}' password reset successfully.");
        }
        else
        {
            logger.LogError($"Failed to reset password for admin user '{adminEmail}'. Errors:");
            foreach (var error in resetResult.Errors)
            {
                logger.LogError($"- {error.Description}");
            }
        }
    }
}
