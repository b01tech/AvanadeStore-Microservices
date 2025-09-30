using Inventory.Application.Services.Consumers;
using Inventory.Application.Services.MessageBus;
using Inventory.Application.UseCases.Product;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Application.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<IGetProductUseCase, GetProductUseCase>();
        services.AddScoped<ICreateProductUseCase, CreateProductUseCase>();
        return services;
    }

    public static IServiceCollection AddConsumerServices(this IServiceCollection services)
    {
        services.AddHostedService<StockValidationConsumerService>();
        services.AddHostedService<OrderFinishedConsumerService>();
        return services;
    }

    public static IServiceCollection AddMessageBus(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var settings = new RabbitMqSettings
        {
            Host = configuration["RabbitMQ:Host"] ?? string.Empty,
            Port = int.TryParse(configuration["RabbitMQ:Port"], out var port) ? port : 5672,
            User = configuration["RabbitMQ:User"] ?? string.Empty,
            Password = configuration["RabbitMQ:Password"] ?? string.Empty,
            PrefetchCount = ushort.TryParse(
                configuration["RabbitMQ:PrefetchCount"] ?? string.Empty,
                out var prefetch
            )
                ? prefetch
                : (ushort)10
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
