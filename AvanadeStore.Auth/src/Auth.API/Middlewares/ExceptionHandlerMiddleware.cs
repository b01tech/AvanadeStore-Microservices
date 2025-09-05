using Auth.Application.DTOs.Responses;
using Auth.Exception.CustomExceptions;
using Auth.Exception.ErrorMessages;

namespace Auth.API.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (CustomAppException exception)
        {
            await HandleCustomException(httpContext, exception);
        }
        catch (System.Exception exception)
        {
            await HandleUnknownException(httpContext, exception);
        }
    }

    private async Task HandleUnknownException(HttpContext httpContext, System.Exception exception)
    {
        _logger.LogError(message: exception.Message);

        var status = StatusCodes.Status500InternalServerError;
        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(new ResponseErrorMessage(status, ResourceErrorMessages.UNKNOWN_ERROR));
    }

    private async Task HandleCustomException(HttpContext httpContext, CustomAppException exception)
    {
        _logger.LogError(message: exception.Message);

        if (exception is OnValidationException)
        {
            var status = StatusCodes.Status400BadRequest;
            httpContext.Response.StatusCode = status;
            await httpContext.Response.WriteAsJsonAsync(new ResponseErrorMessage(status, exception.ErrorMessages.ToList()));
        }
    }
}

public static class ExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlerMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlerMiddleware>();
    }
}
