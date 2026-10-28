
using Microsoft.AspNetCore.Mvc;

namespace ClyvoCareOSChallenge.Middleware;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
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
            _logger.LogError(
                ex,
                "Erro não tratado no processamento da requisição {Path}",
                context.Request.Path
            );

            if (context.Response.HasStarted)
                throw;

            context.Response.Clear();

            context.Response.StatusCode =
                StatusCodes.Status500InternalServerError;

            context.Response.ContentType =
                "application/problem+json";

            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Erro interno do servidor",
                Detail = "Ocorreu um erro inesperado. Informe o identificador da requisição ao suporte.",
                Instance = context.Request.Path
            };

            problem.Extensions["traceId"] =
                context.TraceIdentifier;

            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
