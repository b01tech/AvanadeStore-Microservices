using Sales.Domain.Interfaces;
using Sales.Exception.ErrorMessages;
using Sales.Infra.Data;
using Sales.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Sales.Infra.Extensions;
public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddContext(services, configuration);
        AddRepositories(services);
        return services;
    }

    private static void AddContext(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SalesConnection");
        services.AddDbContext<SalesDbContext>(opt =>
            opt.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure()));
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IOrderRepository, OrderRepository>();
    }

    public static void ApplyMigrations(this IHost app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var servicesProvider = scope.ServiceProvider;
            var dbContext = servicesProvider.GetRequiredService<SalesDbContext>();
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