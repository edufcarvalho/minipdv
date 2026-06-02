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
            var errors = ex.Errors.Select(e => e.ErrorMessage).ToArray();
            _logger.LogWarning("Erro de validação na requisição {Method} {Path}. Erros: {Errors}",
                context.Request.Method, context.Request.Path, string.Join("; ", errors));
            await WriteProblemDetails(context, HttpStatusCode.BadRequest, "Validation Error", errors);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Acesso não autorizado à rota {Method} {Path} pelo IP {Ip}. Motivo: {Message}",
                context.Request.Method, context.Request.Path, context.Connection.RemoteIpAddress, ex.Message);
            await WriteProblemDetails(context, HttpStatusCode.Forbidden, "Forbidden", [ex.Message]);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Recurso não encontrado na rota {Method} {Path}. Detalhe: {Message}",
                context.Request.Method, context.Request.Path, ex.Message);
            await WriteProblemDetails(context, HttpStatusCode.NotFound, "Not Found", [ex.Message]);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Operação inválida na rota {Method} {Path}. Detalhe: {Message}",
                context.Request.Method, context.Request.Path, ex.Message);
            await WriteProblemDetails(context, HttpStatusCode.BadRequest, "Bad Request", [ex.Message]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado na requisição {Method} {Path} pelo IP {Ip}. Tipo: {ExceptionType}",
                context.Request.Method, context.Request.Path, context.Connection.RemoteIpAddress, ex.GetType().Name);
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
