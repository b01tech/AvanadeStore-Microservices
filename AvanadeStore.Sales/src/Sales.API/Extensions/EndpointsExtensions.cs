using Sales.Application.DTOs.Requests;
using Sales.Application.DTOs.Responses;
using Sales.Application.UseCases.Order;
using Sales.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Sales.API.Extensions;

public static class EndpointsExtensions
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        MapOrderEndpoints(app);
        return app;
    }

    private static void MapOrderEndpoints(WebApplication app)
    {
        var group = app.MapGroup("/order").WithTags("Order").WithOpenApi();

        group.MapGet("/list/{page:int}", async (IGetOrderUseCase useCase, int page = 1) =>
        {
            var result = await useCase.ExecuteGetAllAsync(page);
            return result;
        }).WithDescription("**Obtém todos os pedidos paginados**🔑 (Role: Employee, Manager)")
            .Produces<ResponseOrdersListDTO>(StatusCodes.Status200OK)
            .RequireAuthorization(policy => policy.RequireRole("Employee", "Manager"));

        group.MapGet("/{id:guid}", async (Guid id, IGetOrderUseCase useCase) =>
        {
            var result = await useCase.ExecuteAsync(id);
            return Results.Ok(result);
        }).WithDescription("**Obtém um pedido pelo ID**🔑 (Role: Employee, Manager)")
        .Produces<ResponseOrderDTO>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .RequireAuthorization(policy => policy.RequireRole("Employee", "Manager"));

        group.MapGet("/status/{status}/{page:int}", async (OrderStatus status, IGetOrderUseCase useCase, int page = 1) =>
        {
            var result = await useCase.ExecuteGetByStatusAsync(status, page);
            return Results.Ok(result);
        }).WithDescription("**Obtém pedidos filtrados por status**🔑 (Role: Employee, Manager)")
            .Produces<ResponseOrdersListDTO>(StatusCodes.Status200OK)
            .RequireAuthorization(policy => policy.RequireRole("Employee", "Manager"));

        group.MapGet("/my/{page:int}", async (IGetOrderUseCase useCase, HttpContext context, int page = 1) =>
        {
            var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            var result = await useCase.ExecuteGetByUserIdAsync(Guid.Parse(userId), page);
            return Results.Ok(result);
        }).WithDescription("**Obtém os pedidos do cliente logado**🔑 (Role: Client)")
            .Produces<ResponseOrdersListDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization(policy => policy.RequireRole("Client"));

        group.MapPost("/", async (ICreateOrderUseCase useCase, RequestCreateOrderDTO request, HttpContext context) =>
        {
            var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            var result = await useCase.ExecuteAsync(request, Guid.Parse(userId));
            return Results.Created(string.Empty, result);
        }).WithDescription("**Cria um novo pedido**🔑 (Role: Client)")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization(policy => policy.RequireRole("Client"));

        group.MapPut("/{id:guid}/confirm-separation", async (Guid id, IUpdateOrderStatusUseCase useCase) =>
        {
            var result = await useCase.ExecuteConfirmSeparationAsync(id);
            return Results.Ok(result);
        }).WithDescription("**Confirma separação do pedido (Confirmed → InSeparation)**🔑 (Role: Employee)")
            .Produces<ResponseOrderDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole("Employee"));

        group.MapPut("/{id:guid}/cancel", async (Guid id, IUpdateOrderStatusUseCase useCase, HttpContext context) =>
        {
            var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            var result = await useCase.ExecuteCancelOrderAsync(id, Guid.Parse(userId));
            return Results.Ok(result);
        }).WithDescription("**Cancela o pedido (Confirmed/InSeparation → Cancelled)**🔑 (Role: Client)")
            .Produces<ResponseOrderDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization(policy => policy.RequireRole("Client"));

        group.MapPut("/{id:guid}/finish", async (Guid id, IUpdateOrderStatusUseCase useCase) =>
        {
            var result = await useCase.ExecuteFinishOrderAsync(id);
            return Results.Ok(result);
        }).WithDescription("**Finaliza o pedido (InSeparation → Finished)**🔑 (Role: Employee)")
            .Produces<ResponseOrderDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole("Employee"));
    }
}
