using System.Text.Json.Serialization;

namespace Sales.API.Extensions;

public static class JsonExtensions
{
    public static IServiceCollection AddJsonConfiguration(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        return services;
    }
}