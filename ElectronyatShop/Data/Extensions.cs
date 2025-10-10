using ElectronyatShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;

namespace ElectronyatShop.Data;

public static class Extensions
{
    public static void AddDatabaseToServices(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("MySqlConnection")!;
        
        builder.Services.AddDbContext<ElectronyatShopDbContext>(
            options =>
            {
                options.UseMySQL(connectionString, o =>
                {
                    o.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null
                    );
                });
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            }
        );
    }
    
    public static void AddDatabaseIdentityToServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddDefaultIdentity<ApplicationUser>(options => 
                options.SignIn.RequireConfirmedAccount = false)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ElectronyatShopDbContext>()
            .AddSignInManager<SignInManager<ApplicationUser>>();
    }

    public static void AddUserRolesToDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("AdminRole", op =>
                op.RequireRole("AdminRole"))
            .AddPolicy("CustomerRole", op =>
                op.RequireRole("CustomerRole"));
    }
    
    public static async Task EnableMigrationsOnStartup(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ElectronyatShopDbContext>();
        await db.Database.MigrateAsync();
    }
    
    public static async Task AddAdminToDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        const string adminRole = "AdminRole";
        const string customerRole = "CustomerRole";
        const string email = "admin@admin.com";

        var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>()
                          ?? throw new NotSupportedException("roleManager null");

        // Create roles if they don't exist
        if (!await roleManager.RoleExistsAsync(adminRole))
            await roleManager.CreateAsync(new IdentityRole(adminRole));
            
        if (!await roleManager.RoleExistsAsync(customerRole))
            await roleManager.CreateAsync(new IdentityRole(customerRole));

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var admin = await userManager.FindByEmailAsync(email);
        if (admin is not null) return;

        admin = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FirstName = "Mohamed",
            LastName = "Al-Adawy",
            Address = "Giza, Egypt",
            PhoneNumber = "01120664373"
        };
        await userManager.CreateAsync(admin, "Admin@123");
        await userManager.AddToRoleAsync(admin, adminRole);
    }
}