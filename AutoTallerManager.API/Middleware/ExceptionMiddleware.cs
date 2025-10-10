using System.Net;
using System.Text.Json;
using AutoTallerManager.API.Helpers.Errors;

namespace AutoTallerManager.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment environment)
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
            _logger.LogError(ex, "Error no manejado: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = context.Response;

        var errorResponse = new ApiError();

        switch (exception)
        {
            case ApplicationException ex:
                if (ex.Message.Contains("No se puede eliminar"))
                {
                    errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Message = ex.Message;
                }
                else
                {
                    errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Message = ex.Message;
                }
                break;
            case KeyNotFoundException ex:
                errorResponse.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Message = ex.Message;
                break;
            case UnauthorizedAccessException ex:
                errorResponse.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Message = ex.Message;
                break;
            case ArgumentException ex:
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = ex.Message;
                break;
            default:
                errorResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Message = _environment.IsDevelopment() 
                    ? exception.Message 
                    : "Error interno del servidor. Contacte al administrador.";
                break;
        }

        _logger.LogError(exception, "Error manejado: {StatusCode} - {Message}", errorResponse.StatusCode, errorResponse.Message);

        var result = JsonSerializer.Serialize(errorResponse);
        await context.Response.WriteAsync(result);
    }
}
