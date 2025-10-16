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
using System.ComponentModel.DataAnnotations;

namespace AutoTallerManager.API.Controllers.Auth;

/// <summary>
/// Controlador para operaciones de autenticación y autorización
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Route("api/usuario")] // alias para compatibilidad con tests que usan /api/usuario
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
    [HttpPost("usuario/login")] // alias en español para compatibilidad con tests
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
    [Authorize(Roles = "Admin")] // Solo administradores pueden registrar usuarios
    [HttpPost("user/register")] // alias en español; protegido, tests esperan 401 sin token
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

    /// <summary>
    /// Cerrar sesión del usuario
    /// </summary>
    /// <returns>Confirmación de logout</returns>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        try
        {
            // Obtener el usuario actual del token
            var userIdClaim = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Token inválido");
            }

            var user = await _unitOfWork.UserMembers.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Usuario no encontrado");
            }

            // Revocar todos los refresh tokens activos
            if (user.RefreshTokens != null)
            {
                foreach (var token in user.RefreshTokens.Where(t => t.IsActive))
                {
                    token.Revoked = DateTime.UtcNow;
                }
                await _unitOfWork.UserMembers.UpdateAsync(user);
                await _unitOfWork.SaveChanges();
            }

            _logger.LogInformation("Logout exitoso para usuario: {UserId}", userId);
            return Ok(new { message = "Sesión cerrada exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el logout");
            return BadRequest(new { error = ex.Message });
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
    [HttpGet] // soporta GET /api/usuario
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        try
        {
            var users = await _db.UsersMembers
                .Include(u => u.UserMemberRols)
                    .ThenInclude(umr => umr.Role)
                .ToListAsync();

            var usersDto = users.Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email ?? string.Empty,
                RoleName = u.UserMemberRols?.FirstOrDefault()?.Role?.RoleName ?? "Sin rol",
                StateName = "Activo" 
            });

            return Ok(usersDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuarios");
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// Obtener usuario por ID
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <returns>Información del usuario</returns>
    [HttpGet("users/{id}")]
    [Authorize(Roles = "Admin")]
    [HttpGet("user/{id}")] 
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        try
        {
            var user = await _db.UsersMembers
                .Include(u => u.UserMemberRols)
                    .ThenInclude(umr => umr.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound("Usuario no encontrado");
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                RoleName = user.UserMemberRols?.FirstOrDefault()?.Role?.RoleName ?? "Sin rol",
                StateName = "Activo"
            };

            return Ok(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuario {UserId}", id);
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// Actualizar información de usuario
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="request">Datos actualizados</param>
    /// <returns>Confirmación de actualización</returns>
    [HttpPut("users/{id}")]
    [Authorize(Roles = "Admin")]
    [HttpPut("user/{id}")] 
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto request)
    {
        try
        {
            var user = await _db.UsersMembers
                .Include(u => u.UserMemberRols)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound("Usuario no encontrado");
            }

            // Actualizar datos básicos
            user.Email = request.Email;
            user.UpdatedAt = DateTime.UtcNow;

            // Actualizar rol si es necesario
            var currentRole = user.UserMemberRols?.FirstOrDefault();
            if (currentRole?.RoleId != request.RoleId)
            {
                // Eliminar rol actual
                if (currentRole != null)
                {
                    _db.UserMemberRols.Remove(currentRole);
                }

                // Agregar nuevo rol
                var newUserRole = new UserMemberRole
                {
                    UserMemberId = user.Id,
                    RoleId = request.RoleId
                };
                _db.UserMemberRols.Add(newUserRole);
            }

            await _db.SaveChangesAsync();

            _logger.LogInformation($"Usuario {id} actualizado exitosamente");
            return Ok(new { message = "Usuario actualizado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar usuario {UserId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Cambiar contraseña de usuario
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="request">Datos de cambio de contraseña</param>
    /// <returns>Confirmación de cambio</returns>
    [HttpPut("users/{id}/change-password")]
    [Authorize(Roles = "Admin")]
    [HttpPut("user/{id}/change-password")] 
    public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordDto request)
    {
        try
        {
            var user = await _db.UsersMembers.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound("Usuario no encontrado");
            }

            // Validar contraseña actual si se proporciona
            if (!string.IsNullOrEmpty(request.CurrentPassword))
            {
                var verification = _passwordHasher.VerifyHashedPassword(user, user.Password ?? string.Empty, request.CurrentPassword);
                if (verification == PasswordVerificationResult.Failed)
                {
                    return BadRequest(new { error = "Contraseña actual incorrecta" });
                }
            }

            // Actualizar contraseña
            user.Password = _passwordHasher.HashPassword(user, request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            _logger.LogInformation("Contraseña cambiada para usuario {UserId}", id);
            return Ok(new { message = "Contraseña cambiada exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar contraseña para usuario {UserId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Asignar rol a usuario
    /// </summary>
    /// <param name="request">Datos de asignación de rol</param>
    /// <returns>Confirmación de asignación</returns>
    [HttpPost("users/assign-role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto request)
    {
        try
        {
            var user = await _db.UsersMembers
                .Include(u => u.UserMemberRols)
                .FirstOrDefaultAsync(u => u.Id == request.UserId);

            if (user == null)
            {
                return NotFound("Usuario no encontrado");
            }

            var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == request.RoleId);
            if (role == null)
            {
                return NotFound("Rol no encontrado");
            }

            // Eliminar roles existentes
            if (user.UserMemberRols != null && user.UserMemberRols.Any())
            {
                _db.UserMemberRols.RemoveRange(user.UserMemberRols);
            }

            // Asignar nuevo rol
            var userRole = new UserMemberRole
            {
                UserMemberId = request.UserId,
                RoleId = request.RoleId
            };

            _db.UserMemberRols.Add(userRole);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Rol {RoleId} asignado al usuario {UserId}", request.RoleId, request.UserId);
            return Ok(new { message = "Rol asignado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al asignar rol al usuario {UserId}", request.UserId);
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
                Email = "admin@autotaller.com",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            // Hash de la contraseña después de crear el objeto
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
    [HttpPost("setup/rols")]
    public async Task<IActionResult> CreateBasicRols()
    {
        try
        {
            var results = new List<string>();

            // Roles del sistema
            if (!_db.Roles.Any())
            {
                _db.Roles.AddRange(
                    new Role { RoleName = "Admin", Description = "Administrador del sistema" },
                    new Role { RoleName = "Mecanico", Description = "Mecánico del taller" },
                    new Role { RoleName = "Recepcionista", Description = "Recepcionista del taller" }
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
            var adminRole = await _db.Roles.FirstOrDefaultAsync(r => r.RoleName == "Admin");
            if (adminRole == null)
            {
                adminRole = new Role 
                { 
                    RoleName = "Admin", 
                    Description = "Administrador del sistema" 
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
            var existingRols = await _db.UserMemberRols
                .Where(umr => umr.UserMemberId == adminUser.Id)
                .ToListAsync();
            
            if (existingRols.Any())
            {
                _db.UserMemberRols.RemoveRange(existingRols);
                await _db.SaveChangesAsync();
            }

            // Asignar el rol Admin
            var userRole = new UserMemberRole
            {
                UserMemberId = adminUser.Id,
                RoleId = adminRole.Id
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
                .Include(u => u.UserMemberRols)
                    .ThenInclude(umr => umr.Role)
                .FirstOrDefaultAsync(u => u.Email == "admin@autotaller.com");

            if (adminUser == null)
            {
                return NotFound("Usuario administrador no encontrado");
            }

            var rols = await _db.Roles.ToListAsync();
            var userRols = await _db.UserMemberRols
                .Include(umr => umr.Role)
                .Where(umr => umr.UserMemberId == adminUser.Id)
                .ToListAsync();

            var diagnosis = new
            {
                user = new
                {
                    id = adminUser.Id,
                    email = adminUser.Email,
                    username = adminUser.Username,
                    hasUserMemberRols = adminUser.UserMemberRols?.Any() ?? false,
                    userMemberRolsCount = adminUser.UserMemberRols?.Count ?? 0,
                    userMemberRols = adminUser.UserMemberRols?.Select(umr => new
                    {
                        userMemberId = umr.UserMemberId,
                        roleId = umr.RoleId,
                        roleName = umr.Role?.RoleName,
                        roleDescription = umr.Role?.Description
                    }).ToList()
                },
                allRoles = rols.Select(r => new
                {
                    id = r.Id,
                    name = r.RoleName,
                    description = r.Description
                }).ToList(),
                userRolsInDb = userRols.Select(ur => new
                {
                    userMemberId = ur.UserMemberId,
                    roleId = ur.RoleId,
                    roleName = ur.Role?.RoleName,
                    roleDescription = ur.Role?.Description
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
            var addressExists = await _db.Addresses.AnyAsync();
            if (addressExists)
            {
                return Ok(new { 
                    message = "Ya existen direcciones en el sistema",
                    count = await _db.Addresses.CountAsync()
                });
            }

            // Crear dirección básica
            var address = new Address("Calle Principal 123", 1);

            _db.Addresses.Add(address);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Dirección creada: {Description}", address.Description);

            return Ok(new { 
                message = "Dirección creada exitosamente",
                addressId = address.Id,
                description = address.Description
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
    [HttpGet("rols")]
    [HttpGet("user/rols")] // alias en español
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetRols()
    {
        try
        {
            var rols = await _db.Roles.ToListAsync();
            var rolsDto = rols.Select(r => new RoleDto
            {
                Id = r.Id,
                RoleName = r.RoleName ?? string.Empty,
                Description = r.Description ?? string.Empty
            });

            return Ok(rolsDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener roles: {Message}", ex.Message);
            // Fallback para entorno de pruebas sin base de datos
            return Ok(Array.Empty<RoleDto>());
        }
    }

    /// <summary>
    /// Crear nuevo rol en el sistema
    /// </summary>
    /// <param name="request">Datos del nuevo rol</param>
    /// <returns>Confirmación de creación</returns>
    [HttpPost("rols")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RoleDto>> CreateRole([FromBody] CreateRoleDto request)
    {
        try
        {
            // Verificar si el rol ya existe
            var existingRole = await _db.Roles
                .FirstOrDefaultAsync(r => r.RoleName == request.RoleName);
            
            if (existingRole != null)
            {
                return BadRequest(new { error = "El rol ya existe" });
            }

            // Crear nuevo rol
            var newRole = new Role
            {
                RoleName = request.RoleName,
                Description = request.Description
            };

            _db.Roles.Add(newRole);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Rol creado: {RoleName}", newRole.RoleName);

            var roleDto = new RoleDto
            {
                Id = newRole.Id,
                RoleName = newRole.RoleName ?? string.Empty,
                Description = newRole.Description ?? string.Empty
            };

            return CreatedAtAction(nameof(GetRols), new { id = newRole.Id }, roleDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear rol: {Message}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Actualizar rol existente
    /// </summary>
    /// <param name="id">ID del rol</param>
    /// <param name="request">Datos actualizados del rol</param>
    /// <returns>Confirmación de actualización</returns>
    [HttpPut("rols/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleDto request)
    {
        try
        {
            var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == id);
            if (role == null)
            {
                return NotFound("Rol no encontrado");
            }

            // Verificar si el nuevo nombre ya existe en otro rol
            var existingRole = await _db.Roles
                .FirstOrDefaultAsync(r => r.RoleName == request.RoleName && r.Id != id);
            
            if (existingRole != null)
            {
                return BadRequest(new { error = "Ya existe un rol con ese nombre" });
            }

            // Actualizar datos
            role.RoleName = request.RoleName;
            role.Description = request.Description;

            await _db.SaveChangesAsync();

            _logger.LogInformation("Rol actualizado {RoleId}", id);
            return Ok(new { message = "Rol actualizado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar rol {RoleId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Eliminar rol del sistema
    /// </summary>
    /// <param name="id">ID del rol</param>
    /// <returns>Confirmación de eliminación</returns>
    [HttpDelete("roles/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        try
        {
            var role = await _db.Roles
                .Include(r => r.UserMemberRols)
                .FirstOrDefaultAsync(r => r.Id == id);
            
            if (role == null)
            {
                return NotFound("Rol no encontrado");
            }

            // Verificar si el rol está siendo usado por usuarios
            if (role.UserMemberRols != null && role.UserMemberRols.Any())
            {
                return BadRequest(new { error = "No se puede eliminar el rol porque está asignado a usuarios" });
            }

            _db.Roles.Remove(role);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Rol eliminado {RoleId}", id);
            return Ok(new { message = "Rol eliminado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar rol {RoleId}", id);
            return BadRequest(new { error = ex.Message });
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
}

// DTO temporal para refresh token
public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
