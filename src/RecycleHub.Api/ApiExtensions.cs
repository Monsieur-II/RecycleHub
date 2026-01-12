using Microsoft.EntityFrameworkCore;
using RecycleHub.Api.Services.Interfaces;
using RecycleHub.Api.Services.Providers;
using RecycleHub.Pg.Sdk;

namespace RecycleHub.Api;

public static class ApiExtensions
{
    public static async Task ApplyPendingMigrations(this WebApplication app)
    {
        var scope = app.Services.CreateScope();
        var scopedServiceProvider = scope.ServiceProvider;
        var context = scopedServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        await context.Database.EnsureCreatedAsync();

        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

        if (pendingMigrations.Any())
        {
            await context.Database.MigrateAsync();
        }
    }

    public static void AddBusinessServices(this IServiceCollection services)
    {
        services.AddScoped<IRecyclingCenterService, RecyclingCenterService>();
        services.AddScoped<ILookUpService, LookUpService>();
    }
}
