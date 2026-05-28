using System.Net;
using System.Text.Json;
using FluentValidation;
using SumX.API.Common;
using SumX.Application.Common.Exceptions;

namespace SumX.API.Middlewares;

public sealed class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        HttpStatusCode status;
        string message;
        object? errors = null;

        switch (exception)
        {
            case ValidationException validationEx:
                status = HttpStatusCode.BadRequest;
                message = "Validation failed";
                errors = validationEx.Errors.Select(e => e.ErrorMessage);
                break;

            case UnauthorizedException:
                status = HttpStatusCode.Unauthorized;
                message = exception.Message;
                break;

            case NotFoundException:
                status = HttpStatusCode.NotFound;
                message = exception.Message;
                break;

            default:
                status = HttpStatusCode.InternalServerError;
                message = "An unexpected error occurred";
                break;
        }

        context.Response.StatusCode = (int)status;

        var response = ApiResponse<object>.Fail(message, (int)status);

        var result = JsonSerializer.Serialize(new
        {
            response.Success,
            response.Message,
            response.StatusCode,
            Errors = errors
        });

        await context.Response.WriteAsync(result);
    }
}