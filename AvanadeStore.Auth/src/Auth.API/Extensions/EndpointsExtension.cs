using Auth.Application.DTOs.Requests;
using Auth.Application.UseCases.Client;

namespace Auth.API.Extensions;

public static class EndpointsExtension
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        MapClientEndpoints(app);
        return app;
    }

    private static void MapClientEndpoints(WebApplication app)
    {
        var group = app.MapGroup("/client").WithTags("Client").WithOpenApi();

        app.MapPost("/", async (RequestCreateClientDTO request, ICreateClientUseCase useCase) =>
        {
            var result = await useCase.ExecuteAsync(request);
            return Results.Ok(result);
        });

    }
}
