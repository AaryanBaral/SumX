using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using FluentValidation;
using SumX.API.Common;
using SumX.Application.Common.Exceptions;
using SumX.Domain.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;

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
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = GetTitle(statusCode),
            Detail = message,
            Instance = context.Request.Path
        };

        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        if (errors is not null)
        {
            problemDetails.Extensions["errors"] = errors;
        }

        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private static string GetTitle(int statusCode) =>
        statusCode switch
        {
            StatusCodes.Status400BadRequest => "Bad Request",
            StatusCodes.Status401Unauthorized => "Unauthorized",
            StatusCodes.Status403Forbidden => "Forbidden",
            StatusCodes.Status404NotFound => "Not Found",
            _ => "Internal Server Error"
        };

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
