using Inventory.Domain.Interfaces;
using Inventory.Exception.ErrorMessages;
using Inventory.Infra.Data;
using Inventory.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Inventory.Infra.Extensions;
public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddContext(services, configuration);
        AddRepositories(services);
        return services;
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProductRepository, ProductRepository>();
    }

    private static void AddContext(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("InventoryConnection");
        services.AddDbContext<InventoryDbContext>(opt =>
            opt.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure()));
    }

    public static void ApplyMigrations(this IHost app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var servicesProvider = scope.ServiceProvider;
            var dbContext = servicesProvider.GetRequiredService<InventoryDbContext>();
            try
            {
                if (dbContext.Database.GetPendingMigrations().Any())
                    dbContext.Database.Migrate();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"{ResourceErrorMessages.DB_CONNECTION_FAIL}:{ex.Message}");
            }
        }
    }
}
