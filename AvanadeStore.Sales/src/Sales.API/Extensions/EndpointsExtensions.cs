namespace Sales.API.Extensions;

public static class EndpointsExtensions
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        MapOrder(app);
        return app;
    }

    private static void MapOrder(WebApplication app)
    {
        var group = app.MapGroup("/order").WithTags("Order").WithOpenApi();
    }
}
