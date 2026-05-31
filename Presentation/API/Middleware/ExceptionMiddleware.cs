using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace minipdv.Presentation.API.Middleware;

public class ExceptionMiddleware
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
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error");
            await WriteProblemDetails(context, HttpStatusCode.BadRequest, "Validation Error", ex.Errors.Select(e => e.ErrorMessage).ToArray());
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access");
            await WriteProblemDetails(context, HttpStatusCode.Forbidden, "Forbidden", [ex.Message]);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found");
            await WriteProblemDetails(context, HttpStatusCode.NotFound, "Not Found", [ex.Message]);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation");
            await WriteProblemDetails(context, HttpStatusCode.BadRequest, "Bad Request", [ex.Message]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteProblemDetails(context, HttpStatusCode.InternalServerError, "Internal Server Error", ["An unexpected error occurred."]);
        }
    }

    private static async Task WriteProblemDetails(HttpContext context, HttpStatusCode statusCode, string title, string[] errors)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var problem = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = errors.Length == 1 ? errors[0] : null,
            Instance = context.Request.Path
        };

        if (errors.Length > 0)
            problem.Extensions["errors"] = errors;

        var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await context.Response.WriteAsync(json);
    }
}
