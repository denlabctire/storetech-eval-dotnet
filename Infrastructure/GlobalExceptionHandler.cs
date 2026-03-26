using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using storetech_eval_dotnet.Services;

namespace storetech_eval_dotnet.Infrastructure;

public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, type) = MapException(exception);

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception while processing request.");
        }
        else
        {
            logger.LogWarning(exception, "Request failed validation or referenced missing data.");
        }

        httpContext.Response.StatusCode = statusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = exception.Message,
                Type = type
            }
        });
    }

    private static (int StatusCode, string Title, string Type) MapException(Exception exception)
    {
        return exception switch
        {
            CartNotFoundException or ProductNotFoundException or KeyNotFoundException =>
                (StatusCodes.Status404NotFound, "Resource not found.", "https://httpstatuses.com/404"),
            ArgumentOutOfRangeException or UnsupportedCurrencyException or TaxConfigurationException or CartScopeMismatchException =>
                (StatusCodes.Status400BadRequest, "The request is invalid.", "https://httpstatuses.com/400"),
            _ =>
                (StatusCodes.Status500InternalServerError, "An unexpected error occurred.", "https://httpstatuses.com/500")
        };
    }
}