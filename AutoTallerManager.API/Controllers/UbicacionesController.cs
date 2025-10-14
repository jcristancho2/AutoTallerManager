using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using AutoTallerManager.Infrastructure.Persistence.Context;

namespace AutoTallerManager.API.Controllers;

/// <summary>
/// Controlador para gestión de ubicaciones geográficas
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UbicacionesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AppDbContext _db;
    private readonly ILogger<UbicacionesController> _logger;

    public UbicacionesController(IUnitOfWork unitOfWork, AppDbContext db, ILogger<UbicacionesController> logger)
    {
        _unitOfWork = unitOfWork;
        _db = db;
        _logger = logger;
    }

    #region Países

    /// <summary>
    /// Obtener todos los países
    /// </summary>
    /// <returns>Lista de países</returns>
    [HttpGet("paises")]
    public async Task<ActionResult<IEnumerable<PaisDto>>> GetPaises()
    {
        try
        {
            var paises = await _db.Paises.ToListAsync();
            var paisesDto = paises.Select(p => new PaisDto
            {
                Id = p.Id,
                Nombre = p.Nombre ?? string.Empty
            });

            return Ok(paisesDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener países");
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// Crear nuevo país
    /// </summary>
    /// <param name="request">Datos del país</param>
    /// <returns>País creado</returns>
    [HttpPost("paises")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PaisDto>> CreatePais([FromBody] CreatePaisDto request)
    {
        try
        {
            // Verificar si el país ya existe
            var existingPais = await _db.Paises
                .FirstOrDefaultAsync(p => p.Nombre == request.Nombre);
            
            if (existingPais != null)
            {
                return BadRequest(new { error = "El país ya existe" });
            }

            var pais = new Pais(request.Nombre);
            _db.Paises.Add(pais);
            await _db.SaveChangesAsync();

            _logger.LogInformation("País creado: {Nombre}", pais.Nombre);

            var paisDto = new PaisDto
            {
                Id = pais.Id,
                Nombre = pais.Nombre ?? string.Empty
            };

            return CreatedAtAction(nameof(GetPaises), new { id = pais.Id }, paisDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear país");
            return BadRequest(new { error = ex.Message });
        }
    }

    #endregion

    #region Departamentos

    /// <summary>
    /// Obtener departamentos por país
    /// </summary>
    /// <param name="paisId">ID del país</param>
    /// <returns>Lista de departamentos</returns>
    [HttpGet("departamentos")]
    public async Task<ActionResult<IEnumerable<DepartamentoDto>>> GetDepartamentos([FromQuery] int? paisId = null)
    {
        try
        {
            var query = _db.Departamentos.Include(d => d.Pais).AsQueryable();
            
            if (paisId.HasValue)
            {
                query = query.Where(d => d.PaisId == paisId.Value);
            }

            var departamentos = await query.ToListAsync();
            var departamentosDto = departamentos.Select(d => new DepartamentoDto
            {
                Id = d.Id,
                Nombre = d.Nombre ?? string.Empty,
                PaisId = d.PaisId,
                PaisNombre = d.Pais?.Nombre ?? string.Empty
            });

            return Ok(departamentosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener departamentos");
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// Crear nuevo departamento
    /// </summary>
    /// <param name="request">Datos del departamento</param>
    /// <returns>Departamento creado</returns>
    [HttpPost("departamentos")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<DepartamentoDto>> CreateDepartamento([FromBody] CreateDepartamentoDto request)
    {
        try
        {
            // Verificar que el país existe
            var pais = await _db.Paises.FirstOrDefaultAsync(p => p.Id == request.PaisId);
            if (pais == null)
            {
                return BadRequest(new { error = "El país especificado no existe" });
            }

            // Verificar si el departamento ya existe en el país
            var existingDepartamento = await _db.Departamentos
                .FirstOrDefaultAsync(d => d.Nombre == request.Nombre && d.PaisId == request.PaisId);
            
            if (existingDepartamento != null)
            {
                return BadRequest(new { error = "El departamento ya existe en este país" });
            }

            var departamento = new Departamento(request.Nombre, request.PaisId);
            _db.Departamentos.Add(departamento);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Departamento creado: {Nombre} en {Pais}", departamento.Nombre, pais.Nombre);

            var departamentoDto = new DepartamentoDto
            {
                Id = departamento.Id,
                Nombre = departamento.Nombre ?? string.Empty,
                PaisId = departamento.PaisId,
                PaisNombre = pais.Nombre ?? string.Empty
            };

            return CreatedAtAction(nameof(GetDepartamentos), new { id = departamento.Id }, departamentoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear departamento");
            return BadRequest(new { error = ex.Message });
        }
    }

    #endregion

    #region Ciudades

    /// <summary>
    /// Obtener ciudades por departamento
    /// </summary>
    /// <param name="departamentoId">ID del departamento</param>
    /// <returns>Lista de ciudades</returns>
    [HttpGet("ciudades")]
    public async Task<ActionResult<IEnumerable<CiudadDto>>> GetCiudades([FromQuery] int? departamentoId = null)
    {
        try
        {
            var query = _db.Ciudades
                .Include(c => c.Departamento)
                    .ThenInclude(d => d.Pais)
                .AsQueryable();
            
            if (departamentoId.HasValue)
            {
                query = query.Where(c => c.Departamento_Id == departamentoId.Value);
            }

            var ciudades = await query.ToListAsync();
            var ciudadesDto = ciudades.Select(c => new CiudadDto
            {
                Id = c.Id,
                Nombre = c.Nombre ?? string.Empty,
                DepartamentoId = c.Departamento_Id,
                DepartamentoNombre = c.Departamento?.Nombre ?? string.Empty,
                PaisNombre = c.Departamento?.Pais?.Nombre ?? string.Empty
            });

            return Ok(ciudadesDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener ciudades");
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// Crear nueva ciudad
    /// </summary>
    /// <param name="request">Datos de la ciudad</param>
    /// <returns>Ciudad creada</returns>
    [HttpPost("ciudades")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CiudadDto>> CreateCiudad([FromBody] CreateCiudadDto request)
    {
        try
        {
            // Verificar que el departamento existe
            var departamento = await _db.Departamentos
                .Include(d => d.Pais)
                .FirstOrDefaultAsync(d => d.Id == request.DepartamentoId);
            
            if (departamento == null)
            {
                return BadRequest(new { error = "El departamento especificado no existe" });
            }

            // Verificar si la ciudad ya existe en el departamento
            var existingCiudad = await _db.Ciudades
                .FirstOrDefaultAsync(c => c.Nombre == request.Nombre && c.Departamento_Id == request.DepartamentoId);
            
            if (existingCiudad != null)
            {
                return BadRequest(new { error = "La ciudad ya existe en este departamento" });
            }

            var ciudad = new Ciudad(request.Nombre, request.DepartamentoId);
            _db.Ciudades.Add(ciudad);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Ciudad creada: {Nombre} en {Departamento}, {Pais}", 
                ciudad.Nombre, departamento.Nombre, departamento.Pais?.Nombre);

            var ciudadDto = new CiudadDto
            {
                Id = ciudad.Id,
                Nombre = ciudad.Nombre ?? string.Empty,
                DepartamentoId = ciudad.Departamento_Id,
                DepartamentoNombre = departamento.Nombre ?? string.Empty,
                PaisNombre = departamento.Pais?.Nombre ?? string.Empty
            };

            return CreatedAtAction(nameof(GetCiudades), new { id = ciudad.Id }, ciudadDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear ciudad");
            return BadRequest(new { error = ex.Message });
        }
    }

    #endregion

    #region Direcciones

    /// <summary>
    /// Obtener direcciones por ciudad
    /// </summary>
    /// <param name="ciudadId">ID de la ciudad</param>
    /// <returns>Lista de direcciones</returns>
    [HttpGet("direcciones")]
    public async Task<ActionResult<IEnumerable<DireccionDto>>> GetDirecciones([FromQuery] int? ciudadId = null)
    {
        try
        {
            var query = _db.Direcciones
                .Include(d => d.Ciudad)
                    .ThenInclude(c => c.Departamento)
                        .ThenInclude(dep => dep.Pais)
                .AsQueryable();
            
            if (ciudadId.HasValue)
            {
                query = query.Where(d => d.CiudadId == ciudadId.Value);
            }

            var direcciones = await query.ToListAsync();
            var direccionesDto = direcciones.Select(d => new DireccionDto
            {
                Id = d.Id,
                Descripcion = d.Descripcion ?? string.Empty,
                CiudadId = d.CiudadId,
                CiudadNombre = d.Ciudad?.Nombre ?? string.Empty,
                DepartamentoNombre = d.Ciudad?.Departamento?.Nombre ?? string.Empty,
                PaisNombre = d.Ciudad?.Departamento?.Pais?.Nombre ?? string.Empty
            });

            return Ok(direccionesDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener direcciones");
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// Crear nueva dirección
    /// </summary>
    /// <param name="request">Datos de la dirección</param>
    /// <returns>Dirección creada</returns>
    [HttpPost("direcciones")]
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<ActionResult<DireccionDto>> CreateDireccion([FromBody] CreateDireccionDto request)
    {
        try
        {
            // Verificar que la ciudad existe
            var ciudad = await _db.Ciudades
                .Include(c => c.Departamento)
                    .ThenInclude(d => d.Pais)
                .FirstOrDefaultAsync(c => c.Id == request.CiudadId);
            
            if (ciudad == null)
            {
                return BadRequest(new { error = "La ciudad especificada no existe" });
            }

            var direccion = new Direccion(request.Descripcion, request.CiudadId);
            _db.Direcciones.Add(direccion);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Dirección creada: {Descripcion} en {Ciudad}, {Departamento}, {Pais}", 
                direccion.Descripcion, ciudad.Nombre, ciudad.Departamento?.Nombre, ciudad.Departamento?.Pais?.Nombre);

            var direccionDto = new DireccionDto
            {
                Id = direccion.Id,
                Descripcion = direccion.Descripcion ?? string.Empty,
                CiudadId = direccion.CiudadId,
                CiudadNombre = ciudad.Nombre ?? string.Empty,
                DepartamentoNombre = ciudad.Departamento?.Nombre ?? string.Empty,
                PaisNombre = ciudad.Departamento?.Pais?.Nombre ?? string.Empty
            };

            return CreatedAtAction(nameof(GetDirecciones), new { id = direccion.Id }, direccionDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear dirección");
            return BadRequest(new { error = ex.Message });
        }
    }

    #endregion

    #region Setup Inicial

    /// <summary>
    /// Crear datos básicos de ubicación (Colombia)
    /// </summary>
    /// <returns>Confirmación de creación</returns>
    [HttpPost("setup/colombia")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetupColombia()
    {
        try
        {
            var results = new List<string>();

            // Crear país Colombia si no existe
            var colombia = await _db.Paises.FirstOrDefaultAsync(p => p.Nombre == "Colombia");
            if (colombia == null)
            {
                colombia = new Pais("Colombia");
                _db.Paises.Add(colombia);
                await _db.SaveChangesAsync();
                results.Add("País Colombia creado");
            }

            // Crear departamentos principales
            var departamentos = new[]
            {
                "Antioquia", "Cundinamarca", "Valle del Cauca", "Atlántico", "Santander",
                "Bolívar", "Nariño", "Córdoba", "Cesar", "Tolima"
            };

            foreach (var deptNombre in departamentos)
            {
                var departamento = await _db.Departamentos
                    .FirstOrDefaultAsync(d => d.Nombre == deptNombre && d.PaisId == colombia.Id);
                
                if (departamento == null)
                {
                    departamento = new Departamento(deptNombre, colombia.Id);
                    _db.Departamentos.Add(departamento);
                    results.Add($"Departamento {deptNombre} creado");
                }
            }

            await _db.SaveChangesAsync();

            // Crear ciudades principales
            var ciudades = new Dictionary<string, string[]>
            {
                ["Antioquia"] = new[] { "Medellín", "Bello", "Itagüí", "Envigado" },
                ["Cundinamarca"] = new[] { "Bogotá", "Soacha", "Zipaquirá", "Chía" },
                ["Valle del Cauca"] = new[] { "Cali", "Palmira", "Buenaventura", "Tuluá" },
                ["Atlántico"] = new[] { "Barranquilla", "Soledad", "Malambo", "Puerto Colombia" },
                ["Santander"] = new[] { "Bucaramanga", "Floridablanca", "Girón", "Piedecuesta" }
            };

            foreach (var (deptNombre, ciudadNombres) in ciudades)
            {
                var departamento = await _db.Departamentos
                    .FirstOrDefaultAsync(d => d.Nombre == deptNombre && d.PaisId == colombia.Id);

                if (departamento != null)
                {
                    foreach (var ciudadNombre in ciudadNombres)
                    {
                        var ciudad = await _db.Ciudades
                            .FirstOrDefaultAsync(c => c.Nombre == ciudadNombre && c.Departamento_Id == departamento.Id);
                        
                        if (ciudad == null)
                        {
                            ciudad = new Ciudad(ciudadNombre, departamento.Id);
                            _db.Ciudades.Add(ciudad);
                            results.Add($"Ciudad {ciudadNombre} creada en {deptNombre}");
                        }
                    }
                }
            }

            await _db.SaveChangesAsync();

            _logger.LogInformation("Datos básicos de Colombia inicializados");

            return Ok(new { 
                message = "Datos básicos de Colombia inicializados exitosamente",
                operations = results,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al inicializar datos de Colombia");
            return BadRequest(new { error = ex.Message });
        }
    }

    #endregion
}

// DTOs para ubicaciones
public class PaisDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class CreatePaisDto
{
    public string Nombre { get; set; } = string.Empty;
}

public class DepartamentoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int PaisId { get; set; }
    public string PaisNombre { get; set; } = string.Empty;
}

public class CreateDepartamentoDto
{
    public string Nombre { get; set; } = string.Empty;
    public int PaisId { get; set; }
}

public class CiudadDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int DepartamentoId { get; set; }
    public string DepartamentoNombre { get; set; } = string.Empty;
    public string PaisNombre { get; set; } = string.Empty;
}

public class CreateCiudadDto
{
    public string Nombre { get; set; } = string.Empty;
    public int DepartamentoId { get; set; }
}

public class DireccionDto
{
    public int Id { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public int CiudadId { get; set; }
    public string CiudadNombre { get; set; } = string.Empty;
    public string DepartamentoNombre { get; set; } = string.Empty;
    public string PaisNombre { get; set; } = string.Empty;
}

public class CreateDireccionDto
{
    public string Descripcion { get; set; } = string.Empty;
    public int CiudadId { get; set; }
}
