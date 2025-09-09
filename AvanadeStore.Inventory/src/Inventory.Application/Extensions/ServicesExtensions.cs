using Inventory.Application.UseCases.Product;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Application.Extensions;
public static class ServicesExtensions
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<IGetProductUseCase, GetProductUseCase>();
        return services;
    }
}
