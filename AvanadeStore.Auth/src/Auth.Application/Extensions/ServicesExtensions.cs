using Auth.Application.Services.Criptography;
using Auth.Application.Services.Mapping;
using Auth.Application.UseCases.Client;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Application.Extensions;
public static class ServicesExtensions
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<ICreateClientUseCase, CreateClientUseCase>();

        return services;
    }
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IEncrypter, Encrypter>();
        services.RegisterMapster();        
        return services;
    }
}
