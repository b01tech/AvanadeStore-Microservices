using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

namespace Inventory.API.Extensions;

public static class DocumentationExtensions
{
    public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi("v1", opt =>
        {
            opt.AddDocumentTransformer((doc, context, cancellationToken) =>
            {
                doc.Info = new OpenApiInfo
                {
                    Title = "Auth API - Avanade Store",
                    Version = "v1",
                    Description = "API de autenticação",
                };
                return Task.CompletedTask;
            });
        });
        return services;
    }
    public static WebApplication MapApiDocumentation(this WebApplication app)
    {
        if (app.Environment.IsDevelopment() == false)
            return app;

        app.MapOpenApi();
        app.MapScalarApiReference(opt =>
        {
            opt.Title = "Avanade Store";
            opt.Theme = ScalarTheme.Solarized;
            opt.DarkMode = true;
        });
        return app;
    }
}
