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
app.MapDefaultControllerRoute();

// Seed Roles and Admin User
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await SeedRolesAndAdminUser(userManager, roleManager, logger);
}

app.Run();

async Task SeedRolesAndAdminUser(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ILogger logger)
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

    string adminEmail = "admin@gmail.com";
    string adminPassword = "admin123!";

    // Create the Admin user if it doesn't exist
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
            logger.LogError($"Failed to create admin user '{adminEmail}'.");
        }
    }
    else
    {
        logger.LogInformation($"Admin user '{adminEmail}' already exists.");
    }
}
