using Auth.Application.DTOs.Requests;
using Auth.Application.DTOs.Responses;
using Auth.Application.UseCases.Client;
using Auth.Application.UseCases.Employee;
using Auth.Application.UseCases.Login;
using System.Security.Claims;

namespace Auth.API.Extensions;

public static class EndpointsExtension
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        MapClientEndpoints(app);
        MapEmployeeEndpoints(app);
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
        }).WithDescription("**Faz cadastro de clientes**")
        .Produces<ResponseCreateUserDTO>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:guid}", async (Guid id, RequestUpdateClientDTO request, IUpdateClientUseCase useCase) =>
        {
            var result = await useCase.ExecuteAsync(id, request);
            return result ? Results.Ok() : Results.BadRequest();
        }).WithDescription("**Atualiza dados do cliente**")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", async (Guid id, ISoftDeleteClientUseCase useCase) =>
        {
            var result = await useCase.ExecuteAsync(id);
            return result ? Results.NoContent() : Results.BadRequest();
        }).WithDescription("**Soft delete do cliente**")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);
    }

    private static void MapEmployeeEndpoints(WebApplication app)
    {
        var group = app.MapGroup("/employee").WithTags("Employee").WithOpenApi();

        group.MapPost("/", async (RequestCreateEmployeeDTO request, ICreateEmployeeUseCase useCase) =>
        {
            var result = await useCase.ExecuteAsync(request);
            return Results.Created(string.Empty, result);
        }).WithDescription("""
            **Faz cadastro de funcionários**

            Valores possíveis para Role:
            - 1: Employee
            - 2: Manager
            """)
        .Produces(StatusCodes.Status201Created).Produces(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:guid}", async (Guid id, RequestUpdateEmployeeDTO request, IUpdateEmployeeUseCase useCase) =>
        {
            var result = await useCase.ExecuteAsync(id, request);
            return result ? Results.Ok() : Results.BadRequest();
        }).WithDescription("**Atualiza dados do funcionário**")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", async (Guid id, ISoftDeleteEmployeeUseCase useCase, ClaimsPrincipal user) =>
        {
            // Obter o ID do usuário autenticado
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var requestingUserId))
                return Results.Unauthorized();

            var result = await useCase.ExecuteAsync(id, requestingUserId);
            return result ? Results.NoContent() : Results.BadRequest();
        }).WithDescription("**Soft delete do funcionário (somente Manager)**")
        .RequireAuthorization()
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);
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
