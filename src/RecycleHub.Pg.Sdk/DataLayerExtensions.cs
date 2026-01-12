using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using RecycleHub.Pg.Sdk.Entities;
using RecycleHub.Pg.Sdk.Repositories.Interfaces;
using RecycleHub.Pg.Sdk.Repositories.Providers;

namespace RecycleHub.Pg.Sdk;

public static class DataLayerExtensions
{
    public static void AddPostgres(this IServiceCollection services,
        IConfiguration configuration, string connectionString,
        ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(configuration.GetConnectionString(connectionString));
        dataSourceBuilder.EnableDynamicJson();
        var npgsqlDataSource = dataSourceBuilder.Build();

        services.AddDbContext<ApplicationDbContext>((_, options) =>
        {
            options.UseNpgsql(npgsqlDataSource,
                    opts => { opts.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery); })
                .EnableSensitiveDataLogging();
        }, serviceLifetime);
        
        
        services.AddScoped<IPgRepository<RecycleCenter>, PgRepository<RecycleCenter>>();
        services.AddScoped<IPgRepository<Material>, PgRepository<Material>>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}
