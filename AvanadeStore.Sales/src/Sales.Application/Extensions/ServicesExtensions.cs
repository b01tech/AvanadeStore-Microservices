using Sales.Application.Services.Consumers;
using Sales.Application.UseCases.Order;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Sales.Application.Services.MessageBus;

namespace Sales.Application.Extensions;
public static class ServicesExtensions
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<IGetOrderUseCase, GetOrderUseCase>();
        services.AddScoped<ICreateOrderUseCase, CreateOrderUseCase>();
        return services;
    }

    public static IServiceCollection AddConsumerServices(this IServiceCollection services)
    {
        services.AddHostedService<OrderValidationConsumerService>();
        return services;
    }

    public static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = new RabbitMqSettings {
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
