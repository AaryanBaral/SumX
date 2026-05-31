using FluentValidation;
using SumX.API.Common;
using SumX.Application.Common.Exceptions;
using SumX.Domain.Exceptions;

namespace SumX.API.Middlewares;

public sealed class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception on {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await WriteErrorAsync(context, ex);
        }
    }

    private async Task WriteErrorAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message, errors) = MapException(exception);

        if (_environment.IsDevelopment() && statusCode >= StatusCodes.Status500InternalServerError)
        {
            message = exception.Message;
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = ApiResponse<object>.Fail(message, statusCode, errors);
        await context.Response.WriteAsJsonAsync(response);
    }

    private static (int StatusCode, string Message, object? Errors) MapException(Exception exception) =>
        exception switch
        {
            ValidationException ex => (
                StatusCodes.Status400BadRequest,
                "Validation failed",
                ex.Errors.Select(e => e.ErrorMessage).ToArray()),

            AppValidationException ex => (
                StatusCodes.Status400BadRequest,
                ex.Message,
                ex.Errors),

            DomainException ex => (
                StatusCodes.Status400BadRequest,
                ex.Message,
                null),

            UnauthorizedException ex => (
                StatusCodes.Status401Unauthorized,
                ex.Message,
                null),

            ForbiddenException ex => (
                StatusCodes.Status403Forbidden,
                ex.Message,
                null),

            NotFoundException ex => (
                StatusCodes.Status404NotFound,
                ex.Message,
                null),

            InvalidOperationException ex when IsMissingTenant(ex) => (
                StatusCodes.Status403Forbidden,
                ex.Message,
                null),

            _ => (
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.",
                null)
        };

    private static bool IsMissingTenant(InvalidOperationException exception) =>
        exception.Message.Contains("Tenant context", StringComparison.OrdinalIgnoreCase)
        || exception.Message.Contains("could not be found", StringComparison.OrdinalIgnoreCase);
}
