using Inventory.Application.Services.MessageBus;
using Inventory.Application.UseCases.Product;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sales.Application.Services.MessageBus;

namespace Inventory.Application.Extensions;
public static class ServicesExtensions
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<IGetProductUseCase, GetProductUseCase>();
        services.AddScoped<ICreateProductUseCase, CreateProductUseCase>();
        return services;
    }
    public static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = new RabbitMqSettings
        {
            Host = configuration["RabbitMq:Host"] ?? string.Empty,
            Port = int.TryParse(configuration["RabbitMq:Port"], out var port) ? port : 5672,
            User = configuration["RabbitMq:User"] ?? string.Empty,
            Password = configuration["RabbitMq:Password"] ?? string.Empty,
            PrefetchCount = ushort.TryParse(configuration["RabbitMq:PrefetchCount"] ?? string.Empty, out var prefetch) ? prefetch : (ushort)10
        };
        services.AddSingleton<IMessageBus>(sp =>
        {
            var bus = new RabbitMqMessageBus(settings);
            bus.InitializeAsync().GetAwaiter().GetResult();
            return bus;
        });

        return services;
    }
}
