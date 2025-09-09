using Auth.Domain.Interfaces;
using Auth.Exception.ErrorMessages;
using Auth.Infra.Data;
using Auth.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Auth.Infra.Extensions;
public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        AddContext(services, config);
        AddRepositories(services);
        return services;
    }

    private static void AddContext(IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AuthDbContext>(opt =>
        {
            var connectionString = config.GetConnectionString("AuthConnection");
            opt.UseSqlServer(connectionString,
            sqlOptions => sqlOptions.EnableRetryOnFailure());
        });
    }
    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
    }

    public static void ApplyMigrations(this IHost app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var servicesProvider = scope.ServiceProvider;
            var dbContext = servicesProvider.GetRequiredService<AuthDbContext>();
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
