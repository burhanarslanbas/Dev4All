using Dev4All.Domain.Exceptions;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace Dev4All.WebAPI.Middlewares;

/// <summary>Catches unhandled exceptions and returns a structured JSON error response.</summary>
public partial class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        switch (exception)
        {
            case ValidationException or BusinessRuleViolationException:
                LogBusinessValidationError(logger, exception.Message, exception);
                break;
            case ResourceNotFoundException:
                LogNotFound(logger, exception.Message, exception);
                break;
            default:
                LogUnhandledError(logger, exception.Message, exception);
                break;
        }

        context.Response.ContentType = "application/json";

        var (statusCode, body) = exception switch
        {
            ValidationException ve => (HttpStatusCode.BadRequest, (object)new
            {
                statusCode = 400,
                timestamp = DateTime.UtcNow,
                path = context.Request.Path.ToString(),
                message = "Validation failed",
                errors = ve.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage })
            }),
            ResourceNotFoundException => (HttpStatusCode.NotFound, (object)new
            {
                statusCode = 404,
                timestamp = DateTime.UtcNow,
                path = context.Request.Path.ToString(),
                message = exception.Message
            }),
            BusinessRuleViolationException => (HttpStatusCode.BadRequest, (object)new
            {
                statusCode = 400,
                timestamp = DateTime.UtcNow,
                path = context.Request.Path.ToString(),
                message = exception.Message
            }),
            UnauthorizedDomainException => (HttpStatusCode.Forbidden, (object)new
            {
                statusCode = 403,
                timestamp = DateTime.UtcNow,
                path = context.Request.Path.ToString(),
                message = exception.Message
            }),
            _ => (HttpStatusCode.InternalServerError, (object)new
            {
                statusCode = 500,
                timestamp = DateTime.UtcNow,
                path = context.Request.Path.ToString(),
                message = "An unexpected error occurred."
            })
        };

        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(body, _jsonOptions));
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Business/Validation error: {Message}")]
    private static partial void LogBusinessValidationError(ILogger logger, string message, Exception ex);

    [LoggerMessage(Level = LogLevel.Information, Message = "Not found: {Message}")]
    private static partial void LogNotFound(ILogger logger, string message, Exception ex);

    [LoggerMessage(Level = LogLevel.Error, Message = "Unhandled exception: {Message}")]
    private static partial void LogUnhandledError(ILogger logger, string message, Exception ex);
}
