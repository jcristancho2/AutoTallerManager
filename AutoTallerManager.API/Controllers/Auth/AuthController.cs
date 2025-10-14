using AutoTallerManager.API.DTOs.Auth;
using AutoTallerManager.API.Services.Interfaces.Auth;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Domain.Entities.Auth;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoTallerManager.API.Controllers.Auth;

/// <summary>
/// Controlador para operaciones de autenticación y autorización
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<UserMember> _passwordHasher;
    private readonly AppDbContext _db;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IUserService userService,
        IUnitOfWork unitOfWork,
        IPasswordHasher<UserMember> passwordHasher,
        AppDbContext db,
        ILogger<AuthController> logger)
    {
        _userService = userService;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _db = db;
        _logger = logger;
    }

    #region Autenticación

    /// <summary>
    /// Iniciar sesión en el sistema
    /// </summary>
    /// <param name="request">Credenciales de acceso</param>
    /// <returns>Token JWT y información del usuario</returns>
    [HttpPost("login")]
    public async Task<ActionResult<DataUserDto>> Login([FromBody] LoginDto request)
    {
        try
        {
            var result = await _userService.GetTokenAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el login");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Registrar nuevo usuario
    /// </summary>
    /// <param name="request">Datos del nuevo usuario</param>
    /// <returns>Confirmación de registro</returns>
    [HttpPost("register")]
    public async Task<ActionResult<string>> Register([FromBody] RegisterDto request)
    {
        try
        {
            var result = await _userService.RegisterAsync(request);
            return Ok(new { message = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el registro");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Renovar token de acceso
    /// </summary>
    /// <returns>Nuevo token JWT</returns>
    [HttpPost("refresh-token")]
    public async Task<ActionResult<DataUserDto>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var result = await _userService.RefreshTokenAsync(request.RefreshToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al renovar token");
            return BadRequest(new { error = ex.Message });
        }
    }

    #endregion

    #region Setup y Configuración

    /// <summary>
    /// Crear usuario administrador inicial
    /// </summary>
    /// <returns>Confirmación de creación</returns>
    [HttpPost("setup/admin")]
    public async Task<IActionResult> CreateInitialAdmin()
    {
        try
        {
            // Verificar si ya existe un admin
            var adminExists = await _db.UsersMembers.AnyAsync(u => u.Email == "admin@autotaller.com");
            if (adminExists)
            {
                return Ok(new { 
                    message = "Usuario administrador ya existe",
                    email = "admin@autotaller.com"
                });
            }

            // Crear usuario admin
            var adminUser = new UserMember
            {
                Username = "admin@autotaller.com",
                Email = "admin@autotaller.com",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            adminUser.Password = _passwordHasher.HashPassword(adminUser, "admin123");

            _db.UsersMembers.Add(adminUser);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Usuario administrador inicial creado: {Email}", adminUser.Email);

            return Ok(new { 
                message = "Usuario administrador creado exitosamente",
                email = adminUser.Email,
                password = "admin123"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear usuario administrador inicial");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Crear roles básicos del sistema
    /// </summary>
    /// <returns>Confirmación de creación</returns>
    [HttpPost("setup/roles")]
    public async Task<IActionResult> CreateBasicRoles()
    {
        try
        {
            var results = new List<string>();

            // Roles del sistema
            if (!_db.Roles.Any())
            {
                _db.Roles.AddRange(
                    new Rol { NombreRol = "Admin", Descripcion = "Administrador del sistema" },
                    new Rol { NombreRol = "Mecanico", Descripcion = "Mecánico del taller" },
                    new Rol { NombreRol = "Recepcionista", Descripcion = "Recepcionista del taller" }
                );
                results.Add("Roles del sistema creados");
            }

            await _db.SaveChangesAsync();

            _logger.LogInformation("Roles básicos del sistema inicializados");

            return Ok(new { 
                message = "Roles básicos inicializados exitosamente",
                operations = results,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al inicializar roles básicos del sistema");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Asignar rol Admin al usuario inicial
    /// </summary>
    /// <returns>Confirmación de asignación</returns>
    [HttpPost("setup/assign-admin")]
    public async Task<IActionResult> AssignAdminRole()
    {
        try
        {
            // Crear rol Admin si no existe
            var adminRole = await _db.Roles.FirstOrDefaultAsync(r => r.NombreRol == "Admin");
            if (adminRole == null)
            {
                adminRole = new Rol 
                { 
                    NombreRol = "Admin", 
                    Descripcion = "Administrador del sistema" 
                };
                _db.Roles.Add(adminRole);
                await _db.SaveChangesAsync();
            }

            // Obtener el usuario admin
            var adminUser = await _db.UsersMembers.FirstOrDefaultAsync(u => u.Email == "admin@autotaller.com");
            if (adminUser == null)
            {
                return NotFound("Usuario administrador no encontrado");
            }

            // Eliminar roles existentes del usuario
            var existingRoles = await _db.UserMemberRols
                .Where(umr => umr.UserMemberId == adminUser.Id)
                .ToListAsync();
            
            if (existingRoles.Any())
            {
                _db.UserMemberRols.RemoveRange(existingRoles);
                await _db.SaveChangesAsync();
            }

            // Asignar el rol Admin
            var userRole = new UserMemberRol
            {
                UserMemberId = adminUser.Id,
                RolId = adminRole.Id
            };

            _db.UserMemberRols.Add(userRole);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Rol Admin asignado al usuario: {Email}", adminUser.Email);

            return Ok(new { 
                message = "Rol Admin asignado exitosamente",
                userId = adminUser.Id,
                roleId = adminRole.Id,
                email = adminUser.Email
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al asignar rol Admin al usuario administrador");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Diagnóstico completo del usuario admin
    /// </summary>
    /// <returns>Información detallada del usuario</returns>
    [HttpPost("setup/diagnose-admin")]
    public async Task<IActionResult> DiagnoseAdmin()
    {
        try
        {
            var adminUser = await _db.UsersMembers
                .Include(u => u.UserMemberRoles)
                    .ThenInclude(umr => umr.Rol)
                .FirstOrDefaultAsync(u => u.Email == "admin@autotaller.com");

            if (adminUser == null)
            {
                return NotFound("Usuario administrador no encontrado");
            }

            var roles = await _db.Roles.ToListAsync();
            var userRoles = await _db.UserMemberRols
                .Include(umr => umr.Rol)
                .Where(umr => umr.UserMemberId == adminUser.Id)
                .ToListAsync();

            var diagnosis = new
            {
                user = new
                {
                    id = adminUser.Id,
                    email = adminUser.Email,
                    username = adminUser.Username,
                    hasUserMemberRoles = adminUser.UserMemberRoles?.Any() ?? false,
                    userMemberRolesCount = adminUser.UserMemberRoles?.Count ?? 0,
                    userMemberRoles = adminUser.UserMemberRoles?.Select(umr => new
                    {
                        userMemberId = umr.UserMemberId,
                        roleId = umr.RolId,
                        roleName = umr.Rol?.NombreRol,
                        roleDescription = umr.Rol?.Descripcion
                    }).ToList()
                },
                allRoles = roles.Select(r => new
                {
                    id = r.Id,
                    name = r.NombreRol,
                    description = r.Descripcion
                }).ToList(),
                userRolesInDb = userRoles.Select(ur => new
                {
                    userMemberId = ur.UserMemberId,
                    roleId = ur.RolId,
                    roleName = ur.Rol?.NombreRol,
                    roleDescription = ur.Rol?.Descripcion
                }).ToList(),
                timestamp = DateTime.UtcNow
            };

            return Ok(diagnosis);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en diagnóstico del usuario admin");
            return BadRequest(new { error = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    /// <summary>
    /// Crear dirección básica para pruebas (endpoint temporal)
    /// </summary>
    /// <returns>Confirmación de creación</returns>
    [HttpPost("setup/create-direccion")]
    public async Task<IActionResult> CreateDireccion()
    {
        try
        {
            // Verificar si ya existe una dirección
            var direccionExists = await _db.Direcciones.AnyAsync();
            if (direccionExists)
            {
                return Ok(new { 
                    message = "Ya existen direcciones en el sistema",
                    count = await _db.Direcciones.CountAsync()
                });
            }

            // Crear dirección básica
            var direccion = new Direccion("Calle Principal 123", 1);

            _db.Direcciones.Add(direccion);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Dirección creada: {Descripcion}", direccion.Descripcion);

            return Ok(new { 
                message = "Dirección creada exitosamente",
                direccionId = direccion.Id,
                descripcion = direccion.Descripcion
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear dirección");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Verificar estado del sistema
    /// </summary>
    /// <returns>Estado de conectividad y configuración</returns>
    [HttpGet("health")]
    public async Task<IActionResult> HealthCheck()
    {
        try
        {
            var healthInfo = new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                database = await CheckDatabaseConnection(),
                adminExists = await CheckAdminExists(),
                basicDataExists = CheckBasicDataExists()
            };

            return Ok(healthInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en health check");
            return StatusCode(500, new { status = "unhealthy", error = ex.Message });
        }
    }

    #endregion

    #region Catálogos y Utilidades

    /// <summary>
    /// Obtener roles disponibles del sistema
    /// </summary>
    /// <returns>Lista de roles</returns>
    [HttpGet("roles")]
    public async Task<ActionResult<IEnumerable<RolDto>>> GetRoles()
    {
        try
        {
            var roles = await _db.Roles.ToListAsync();
            var rolesDto = roles.Select(r => new RolDto
            {
                Id = r.Id,
                NombreRol = r.NombreRol ?? string.Empty,
                Descripcion = r.Descripcion ?? string.Empty
            });

            return Ok(rolesDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener roles: {Message}", ex.Message);
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    #endregion

    #region Métodos Privados

    private async Task<bool> CheckDatabaseConnection()
    {
        try
        {
            await _db.Database.CanConnectAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> CheckAdminExists()
    {
        try
        {
            var admin = await _unitOfWork.UserMembers.GetByUserNameAsync("admin@autotaller.com");
            return admin != null;
        }
        catch
        {
            return false;
        }
    }

    private bool CheckBasicDataExists()
    {
        try
        {
            return _db.Roles.Any();
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region Gestión de Usuarios

    /// <summary>
    /// Obtener todos los usuarios del sistema
    /// </summary>
    /// <returns>Lista de usuarios</returns>
    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetAllUsers()
    {
        try
        {
            var usuarios = await _unitOfWork.UserMembers.GetAllAsync();

            var usuariosDto = usuarios.Select(u => new UsuarioDto
            {
                Id = u.Id,
                Email = u.Email ?? string.Empty,
                RolNombre = u.UserMemberRoles?.FirstOrDefault()?.Rol?.NombreRol ?? string.Empty,
                EstadoNombre = "Activo"
            });

            return Ok(usuariosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuarios");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtener usuario por ID
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <returns>Información del usuario</returns>
    [HttpGet("users/{id}")]
    [Authorize]
    public async Task<ActionResult<UsuarioDto>> GetUserById(int id)
    {
        try
        {
            var usuario = await _unitOfWork.UserMembers.GetByIdAsync(id);
            if (usuario == null)
                return NotFound($"Usuario con ID {id} no encontrado");

            var usuarioDto = new UsuarioDto
            {
                Id = usuario.Id,
                Email = usuario.Email ?? string.Empty,
                RolNombre = usuario.UserMemberRoles?.FirstOrDefault()?.Rol?.NombreRol ?? string.Empty,
                EstadoNombre = "Activo"
            };

            return Ok(usuarioDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuario {UsuarioId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    #endregion
}

// DTOs temporales
public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class RolDto
{
    public int Id { get; set; }
    public string NombreRol { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}
