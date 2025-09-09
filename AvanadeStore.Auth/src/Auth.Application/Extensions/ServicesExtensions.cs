using Auth.Application.Services.Criptography;
using Auth.Application.Services.Mapping;
using Auth.Application.Services.Token;
using Auth.Application.UseCases.Client;
using Auth.Application.UseCases.Employee;
using Auth.Application.UseCases.Login;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Application.Extensions;
public static class ServicesExtensions
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<ICreateClientUseCase, CreateClientUseCase>();
        services.AddScoped<ICreateEmployeeUseCase, CreateEmployeeUseCase>();
        services.AddScoped<ILoginUseCase, LoginUseCase>();

        return services;
    }
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IEncrypter, Encrypter>();
        services.RegisterMapster();
        services.AddTransient<ITokenService, TokenService>();
        return services;
    }
}
