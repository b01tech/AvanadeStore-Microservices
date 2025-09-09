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
                    Title = "Inventory API - Avanade Store",
                    Version = "v1",
                    Description = "API de gestão de estoque de produtos",
                };                
                doc.Components = new OpenApiComponents
                {
                    SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
                    {
                        ["Bearer"] = new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.Http,
                            Scheme = "bearer",
                            BearerFormat = "JWT",
                            Description = "Insira o token JWT no formato: Bearer {seu_token}",
                            In = ParameterLocation.Header,
                            Name = "Authorization"
                        }
                    }
                };
                
                doc.SecurityRequirements = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    }
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
