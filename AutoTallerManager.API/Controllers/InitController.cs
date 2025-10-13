using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Infrastructure.Persistence.Context;

namespace AutoTallerManager.API.Controllers;

/// <summary>
/// Controlador simplificado para inicialización básica del sistema
/// NOTA: La funcionalidad principal de autenticación se movió a AuthController
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class InitController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ILogger<InitController> _logger;

    public InitController(AppDbContext db, ILogger<InitController> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Test básico de conectividad del sistema
    /// </summary>
    /// <returns>Estado de conectividad</returns>
    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok(new { 
            message = "Sistema AutoTallerManager funcionando correctamente",
            timestamp = DateTime.UtcNow,
            version = "1.0.0"
        });
    }

    /// <summary>
    /// Verificar estado de la base de datos
    /// </summary>
    /// <returns>Estado de la base de datos</returns>
    [HttpGet("database-status")]
    public async Task<IActionResult> DatabaseStatus()
    {
        try
        {
            var canConnect = await _db.Database.CanConnectAsync();
            var tablesCount = _db.Model.GetEntityTypes().Count();
            
            return Ok(new {
                connected = canConnect,
                tablesCount = tablesCount,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar estado de la base de datos");
            return StatusCode(500, new { 
                connected = false, 
                error = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Obtener información del sistema
    /// </summary>
    /// <returns>Información del sistema</returns>
    [HttpGet("system-info")]
    public IActionResult SystemInfo()
    {
        return Ok(new {
            application = "AutoTallerManager API",
            version = "1.0.0",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
            framework = Environment.Version.ToString(),
            timestamp = DateTime.UtcNow,
            note = "Para autenticación y gestión de usuarios, usar /api/auth"
        });
    }
}
