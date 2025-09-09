
using Inventory.Application.DTOs.Requests;
using Inventory.Application.DTOs.Responses;
using Inventory.Application.UseCases.Product;

namespace Inventory.API.Extensions;

public static class EndpointsExtensions
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        MapProductEndpoints(app);
        return app;
    }

    private static void MapProductEndpoints(WebApplication app)
    {
        var group = app.MapGroup("/product").WithTags("Product").WithOpenApi();

        group.MapGet("/list/{page:int}", async (IGetProductUseCase useCase, int page = 1) =>
        {
            var result = await useCase.ExecuteGetAllAsync(page);
            return result;
        }).WithDescription("**Obtém todos produtos paginados**")
            .Produces<ResponseProductsListDTO>(StatusCodes.Status200OK);

        group.MapGet("/{id:long}", async (long id, IGetProductUseCase useCase) =>
        {
            var result = await useCase.ExecuteAsync(id);
            return Results.Ok(result);
        }).WithDescription("**Obtém um produto pelo ID**")
        .Produces<ResponseProductDTO>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async (ICreateProductUseCase useCase, RequestCreateProductDTO request) =>
        {
            var result = await useCase.ExecuteAsync(request);
            return Results.Created(string.Empty, result);
        }).WithDescription("**Cria um novo produto**")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict);
    }
}
