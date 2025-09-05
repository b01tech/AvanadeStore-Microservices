using Auth.Application.UseCases.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Application.Extensions;
public static class ServicesExtensions
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<ICreateClientUseCase, CreateClientUseCase>();

        return services;
    }
}
