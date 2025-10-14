using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AutoTallerManager.API.Middleware;

public class AuditoriaMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditoriaMiddleware> _logger;

    public AuditoriaMiddleware(RequestDelegate next, ILogger<AuditoriaMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IUnitOfWork unitOfWork)
    {
        // Solo auditar métodos que modifican datos
        if (ShouldAudit(context.Request.Method))
        {
            await AuditarOperacion(context, unitOfWork);
        }

        await _next(context);
    }

    private static bool ShouldAudit(string method)
    {
        return method is "POST" or "PUT" or "PATCH" or "DELETE";
    }

    private async Task AuditarOperacion(HttpContext context, IUnitOfWork unitOfWork)
    {
        try
        {
            var userId = GetUserId(context);
            if (userId == null) return; // No auditar si no hay usuario autenticado

            var entidadAfectada = GetEntidadAfectada(context.Request.Path);
            var accionId = GetAccionId(context.Request.Method);
            var descripcion = GetDescripcionAccion(context);

            var auditoria = new Auditoria
            {
                UsuarioId = userId.Value,
                EntidadAfectada = entidadAfectada,
                AccionId = accionId,
                FechaHora = DateTime.UtcNow,
                DescripcionAccion = descripcion,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await unitOfWork.Auditorias.AddAsync(auditoria);
            await unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar auditoría");
            // No lanzar excepción para no interrumpir el flujo principal
        }
    }

    private static int? GetUserId(HttpContext context)
    {
        var userIdClaim = context.User.FindFirst("uid");
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        return null;
    }

    private static string GetEntidadAfectada(string path)
    {
        // Extraer el nombre de la entidad del path
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length >= 2 && segments[0] == "api")
        {
            return segments[1].ToLowerInvariant();
        }
        return "unknown";
    }

    private static int GetAccionId(string method)
    {
        return method switch
        {
            "POST" => 1, // Crear
            "PUT" or "PATCH" => 2, // Modificar
            "DELETE" => 3, // Eliminar
            _ => 4 // Consultar
        };
    }

    private static string GetDescripcionAccion(HttpContext context)
    {
        var method = context.Request.Method;
        var path = context.Request.Path;
        var user = context.User.Identity?.Name ?? "Usuario anónimo";

        return $"{method} en {path} por {user}";
    }
}


