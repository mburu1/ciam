using System.Net;
using System.Text.Json;
using Ciam.Application.Common.Exceptions;
using Ciam.Contracts.Common;
using FluentValidation;

namespace Ciam.Api;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    IHostEnvironment environment)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleAsync(context, exception);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception exception)
    {
        var (statusCode, errorCode) = exception switch
        {
            ValidationException => (HttpStatusCode.BadRequest, "validation_error"),
            ResourceNotFoundException => (HttpStatusCode.NotFound, "not_found"),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "unauthorized"),
            _ => (HttpStatusCode.InternalServerError, "internal_error")
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception while processing {Method} {Path}", context.Request.Method, context.Request.Path);
        }
        else
        {
            logger.LogWarning(exception, "Request failed with {StatusCode}: {Method} {Path}", (int)statusCode, context.Request.Method, context.Request.Path);
        }

        var message = statusCode == HttpStatusCode.InternalServerError && !environment.IsDevelopment()
            ? "An unexpected error occurred."
            : exception.Message;

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(new ApiErrorResponse
        {
            ErrorCode = errorCode,
            Message = message,
            Details = exception is ValidationException validation
                ? string.Join("; ", validation.Errors.Select(error => $"{error.PropertyName}: {error.ErrorMessage}"))
                : null
        }));
    }
}
