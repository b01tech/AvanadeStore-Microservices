using Auth.Application.DTOs.Requests;
using Auth.Application.UseCases.Client;

namespace Auth.API.Extensions;

public static class EndpointsExtension
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        MapClientEndpoints(app);
        MapLoginEndpoints(app);
        return app;
    }

    private static void MapClientEndpoints(WebApplication app)
    {
        var group = app.MapGroup("/client").WithTags("Client").WithOpenApi();

        group.MapPost("/", async (RequestCreateClientDTO request, ICreateClientUseCase useCase) =>
        {
            var result = await useCase.ExecuteAsync(request);
            return Results.Ok(result);
        });
    }
    private static void MapLoginEndpoints(WebApplication app)
    {
        var group = app.MapGroup("/login").WithTags("Login").WithOpenApi();
        group.MapPost("/cpf", () =>
        {
            return Results.Ok("login by cpf endpoint");
        });
        group.MapPost("/email", () =>
        {
            return Results.Ok("login by email endpoint");
        });
    }
}
