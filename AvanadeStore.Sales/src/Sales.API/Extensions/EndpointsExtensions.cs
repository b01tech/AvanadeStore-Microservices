using Sales.Application.DTOs.Requests;
using Sales.Application.DTOs.Responses;
using Sales.Application.UseCases.Order;
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
        }).WithDescription("**Obtém todos os pedidos paginados**🔑")
            .Produces<ResponseOrdersListDTO>(StatusCodes.Status200OK)
            .RequireAuthorization();

        group.MapGet("/{id:guid}", async (Guid id, IGetOrderUseCase useCase) =>
        {
            var result = await useCase.ExecuteAsync(id);
            return Results.Ok(result);
        }).WithDescription("**Obtém um pedido pelo ID**🔑")
        .Produces<ResponseOrderDTO>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .RequireAuthorization();

        group.MapPost("/", async (ICreateOrderUseCase useCase, RequestCreateOrderDTO request) =>
        {
            var result = await useCase.ExecuteAsync(request);
            return Results.Created(string.Empty, result);
        }).WithDescription("**Cria um novo pedido**🔑")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict)
            .RequireAuthorization();
    }
}
