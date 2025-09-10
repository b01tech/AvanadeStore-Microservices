using Sales.Application.UseCases.Order;
using Microsoft.Extensions.DependencyInjection;

namespace Sales.Application.Extensions;
public static class ServicesExtensions
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<IGetOrderUseCase, GetOrderUseCase>();
        services.AddScoped<ICreateOrderUseCase, CreateOrderUseCase>();
        return services;
    }
}