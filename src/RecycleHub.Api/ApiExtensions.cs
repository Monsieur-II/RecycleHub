using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecycleHub.Api.Services.Interfaces;
using RecycleHub.Api.Services.Providers;
using RecycleHub.Pg.Sdk;

namespace RecycleHub.Api;

public static class ApiExtensions
{
    public static async Task ApplyPendingMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

        if (pendingMigrations.Any())
        {
            await context.Database.MigrateAsync();
        }
    }

    public static async Task SeedRolesAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles = ["Admin", "User"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    public static void AddBusinessServices(this IServiceCollection services)
    {
        services.AddScoped<IRecyclingCenterService, RecyclingCenterService>();
        services.AddScoped<ILookUpService, LookUpService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IFavoriteService, FavoriteService>();
    }
}
