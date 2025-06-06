using FluentValidation;
using ReceitasCulinarias.API.Models;
using System.Net;
using System.Text.Json;

namespace ReceitasCulinarias.API.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocorreu um erro não tratado: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        ErrorResponse errorResponse;

        switch (exception)
        {
            case ValidationException validationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse = new ErrorResponse(context.Response.StatusCode, "Ocorreram um ou mais erros de validação.")
                {
                    Errors = validationException.Errors.Select(e => e.ErrorMessage)
                };
                break;
            // TODO: Adicionar casos para outras exceções customizadas (ex: NotFoundException)
            // case NotFoundException notFoundException:
            //     context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            //     errorResponse = new ErrorResponse(context.Response.StatusCode, notFoundException.Message);
            //     break;
            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var message = _env.IsDevelopment() ? exception.Message : "Ocorreu um erro interno no servidor.";
                errorResponse = new ErrorResponse(context.Response.StatusCode, message);
                if (_env.IsDevelopment())
                {
                    errorResponse.StackTrace = exception.StackTrace;
                }
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Para manter o padrão camelCase no JSON
        });

        return context.Response.WriteAsync(jsonResponse);
    }
}
