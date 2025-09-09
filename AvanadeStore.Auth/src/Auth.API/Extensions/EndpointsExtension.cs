using Auth.Application.DTOs.Requests;
using Auth.Application.UseCases.Client;
using Auth.Application.UseCases.Login;

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
            return Results.Created(string.Empty, result);
        }).WithDescription("Faz cadastro de clientes")
        .Produces(StatusCodes.Status201Created).Produces(StatusCodes.Status400BadRequest);
    }
    private static void MapLoginEndpoints(WebApplication app)
    {
        var group = app.MapGroup("/login").WithTags("Login").WithOpenApi();
        group.MapPost("/cpf", async (RequestLoginByCpfDTO request, ILoginUseCase useCase) =>
        {
            var result = await useCase.LoginByCpf(request);
            return Results.Ok(result);
        }).WithDescription("Realiza login com CPF")
        .Produces(StatusCodes.Status200OK).Produces(StatusCodes.Status400BadRequest).Produces(StatusCodes.Status401Unauthorized);
        group.MapPost("/email", async (RequestLoginByEmailDTO request, ILoginUseCase useCase) =>
        {
            var result = await useCase.LoginByEmail(request);
            return Results.Ok(result);
        }).WithDescription("Realiza login com Email")
        .Produces(StatusCodes.Status200OK).Produces(StatusCodes.Status400BadRequest).Produces(StatusCodes.Status401Unauthorized);
    }
}
