using System;
using Microsoft.AspNetCore.RateLimiting;
using AutoTallerManager.API.DTOs.Auth;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Domain.Entities.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoTallerManager.API.Services;
using AutoTallerManager.API.Services.Interfaces.Auth;
using Microsoft.AspNetCore.Identity;
using AutoTallerManager.Infrastructure.Persistence.Context;

namespace AutoTallerManager.API.Controllers.Auth;

/// <summary>
/// Controlador unificado para autenticación, gestión de usuarios e inicialización del sistema
/// </summary>
[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("Auth")]
public class AuthController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AuthController> _logger;
    private readonly IJwtService _jwtService;
    private readonly IUserService _userService;
    private readonly IPasswordHasher<UserMember> _passwordHasher;
    private readonly AppDbContext _db;

    public AuthController(
        IUnitOfWork unitOfWork, 
        ILogger<AuthController> logger, 
        IJwtService jwtService,
        IUserService userService,
        IPasswordHasher<UserMember> passwordHasher,
        AppDbContext db)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _jwtService = jwtService;
        _userService = userService;
        _passwordHasher = passwordHasher;
        _db = db;
    }

    #region Autenticación y Tokens

    /// <summary>
    /// Iniciar sesión en el sistema
    /// </summary>
    /// <param name="request">Credenciales de login</param>
    /// <returns>Token JWT y información del usuario</returns>
    [HttpPost("login")]
    public async Task<ActionResult<UsuarioLoginResponseDto>> Login([FromBody] LoginUsuarioDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // Usa el servicio de autenticación mejorado
            var loginDto = new LoginDto 
            { 
                Username = request.Email, 
                Password = request.Password 
            };
            
            var tokenResult = await _userService.GetTokenAsync(loginDto);
            
            if (tokenResult == null || string.IsNullOrEmpty(tokenResult.Token))
                return Unauthorized("Credenciales inválidas");

            // Establece el refresh token en cookie
            if (!string.IsNullOrEmpty(tokenResult.RefreshToken))
            {
                SetRefreshTokenInCookie(tokenResult.RefreshToken);
            }

            // Obtiene información del usuario para la respuesta
            var usuario = await _unitOfWork.UserMembers.GetByUserNameAsync(request.Email);
            if (usuario == null)
                return Unauthorized("Usuario no encontrado");

            _logger.LogInformation("Login exitoso para usuario: {Email}", request.Email);

            return Ok(new UsuarioLoginResponseDto
            {
                Id = usuario.Id,
                Email = usuario.Email ?? string.Empty,
                RolNombre = usuario.UserMemberRoles?.FirstOrDefault()?.Rol?.NombreRol ?? string.Empty,
                EstadoNombre = "Activo", 
                Token = tokenResult.Token,
                RefreshToken = tokenResult.RefreshToken
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el login para usuario: {Email}", request.Email);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Registrar nuevo usuario en el sistema
    /// </summary>
    /// <param name="request">Datos del nuevo usuario</param>
    /// <returns>Información del usuario creado</returns>
    [HttpPost("register")]
    [Authorize(Roles = "Admin")] // Solo administradores pueden registrar usuarios
    public async Task<ActionResult<UsuarioDto>> Register([FromBody] CreateUsuarioDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // Usar servicio de registro mejorado
            var registerDto = new RegisterDto
            {
                Username = request.Email,
                Email = request.Email,
                Password = request.Password
            };

            var result = await _userService.RegisterAsync(registerDto);
            
            // Obtiene usuario creado para respuesta detallada
            var usuario = await _unitOfWork.UserMembers.GetByUserNameAsync(request.Email);
            if (usuario == null)
                return BadRequest("Usuario creado pero no encontrado");

            var rol = usuario.UserMemberRoles?.FirstOrDefault()?.Rol;

            _logger.LogInformation("Usuario registrado exitosamente: {Email}", usuario.Email);

            return CreatedAtAction(nameof(GetUserById), new { id = usuario.Id }, new UsuarioDto
            {
                Id = usuario.Id,
                Email = usuario.Email ?? string.Empty,
                RolNombre = rol?.NombreRol ?? string.Empty,
                EstadoNombre = "Activo"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el registro de usuario: {Email}", request.Email);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Renovar token de acceso usando refresh token
    /// </summary>
    /// <returns>Nuevo token JWT</returns>
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        try
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest("Refresh token is missing.");
            }

            var response = await _userService.RefreshTokenAsync(refreshToken);
            
            if (response == null || string.IsNullOrEmpty(response.Token))
                return Unauthorized("Refresh token inválido");

            if (!string.IsNullOrEmpty(response.RefreshToken))
                SetRefreshTokenInCookie(response.RefreshToken);

            _logger.LogInformation("Token renovado exitosamente");
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante la renovación del token");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Cerrar sesión del usuario actual
    /// </summary>
    /// <returns>Confirmación de logout</returns>
    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        try
        {
            // Eliminar refresh token de cookies
            Response.Cookies.Delete("refreshToken");
            
            _logger.LogInformation("Logout exitoso para usuario: {UserId}", User.Identity?.Name);
            return Ok(new { message = "Logout exitoso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el logout");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    #endregion

    #region Inicialización del Sistema

    /// <summary>
    /// Crear usuario administrador inicial (solo para primera configuración)
    /// </summary>
    /// <returns>Información del admin creado</returns>
    [HttpPost("setup/admin")]
    public async Task<IActionResult> CreateInitialAdmin()
    {
        try
        {
            // Verificar si ya existe un admin
            var existingAdmin = await _unitOfWork.UserMembers.GetByUserNameAsync("admin@autotaller.com");
            if (existingAdmin != null)
            {
                return Ok(new { 
                    message = "Usuario administrador ya existe",
                    email = "admin@autotaller.com",
                    exists = true
                });
            }

            // Crear usuario admin inicial
            var adminUser = new UserMember
            {
                Username = "admin@autotaller.com",
                Email = "admin@autotaller.com",
                Password = "admin123", // Password por defecto
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            adminUser.Password = _passwordHasher.HashPassword(adminUser, "admin123");

            await _unitOfWork.UserMembers.AddAsync(adminUser);
            await _unitOfWork.SaveChangesAsync();

            // Asignar rol de Admin (asumiendo que existe en la base de datos)
            var adminRole = await _unitOfWork.Roles.GetAllAsync();
            var adminRoleId = adminRole.FirstOrDefault(r => r.NombreRol == "Admin")?.Id ?? 1;

            var userRole = new UserMemberRol
            {
                UserMemberId = adminUser.Id,
                RolId = adminRoleId
            };

            _db.UserMemberRols.Add(userRole);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Usuario administrador inicial creado: {Email}", adminUser.Email);

            return Ok(new { 
                message = "Usuario administrador creado exitosamente",
                email = "admin@autotaller.com",
                password = "admin123",
                userId = adminUser.Id,
                roleId = adminRoleId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear usuario administrador inicial");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Inicializar datos básicos del sistema (catálogos, tipos, etc.)
    /// </summary>
    /// <returns>Confirmación de inicialización</returns>
    [HttpPost("setup/seed-data")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SeedBasicData()
    {
        try
        {
            var results = new List<string>();

            // Tipos de Cliente
            if (!_db.TiposCliente.Any())
            {
                _db.TiposCliente.AddRange(
                    new TipoCliente("Particular"), 
                    new TipoCliente("Empresa")
                );
                results.Add("Tipos de cliente creados");
            }

            // Tipos de Vehículo
            if (!_db.TiposVehiculo.Any())
            {
                _db.TiposVehiculo.AddRange(
                    new TipoVehiculo { NombreTipoVehiculo = "Automóvil" }, 
                    new TipoVehiculo { NombreTipoVehiculo = "Motocicleta" },
                    new TipoVehiculo { NombreTipoVehiculo = "Camioneta" }
                );
                results.Add("Tipos de vehículo creados");
            }

            // Marcas y Modelos
            if (!_db.MarcasVehiculo.Any())
            {
                var toyota = new MarcaVehiculo { Nombre = "Toyota" };
                var chevrolet = new MarcaVehiculo { Nombre = "Chevrolet" };
                var ford = new MarcaVehiculo { Nombre = "Ford" };
                
                _db.MarcasVehiculo.AddRange(toyota, chevrolet, ford);
                await _db.SaveChangesAsync();
                
                _db.ModelosVehiculo.AddRange(
                    new ModeloVehiculo("Corolla"),
                    new ModeloVehiculo("Camry"),
                    new ModeloVehiculo("Cruze"),
                    new ModeloVehiculo("Focus")
                );
                results.Add("Marcas y modelos creados");
            }

            // Categorías y Fabricantes
            if (!_db.Categorias.Any())
            {
                _db.Categorias.AddRange(
                    new Categoria { NombreCat = "Filtros" },
                    new Categoria { NombreCat = "Frenos" },
                    new Categoria { NombreCat = "Motor" },
                    new Categoria { NombreCat = "Suspensión" }
                );
                results.Add("Categorías creadas");
            }
            
            if (!_db.Fabricantes.Any())
            {
                _db.Fabricantes.AddRange(
                    new Fabricante { NombreFab = "Bosch", Descripcion = "Repuestos automotrices", Telefono = "000", Email = "info@bosch.com" },
                    new Fabricante { NombreFab = "Mann Filter", Descripcion = "Filtros especializados", Telefono = "000", Email = "info@mann-filter.com" },
                    new Fabricante { NombreFab = "Brembo", Descripcion = "Sistemas de frenado", Telefono = "000", Email = "info@brembo.com" }
                );
                results.Add("Fabricantes creados");
            }

            // Tipos de Servicio, Estados y Tipos de Pago
            if (!_db.TiposServicio.Any())
            {
                _db.TiposServicio.AddRange(
                    new TipoServicio { NombreTipoServ = "Mantenimiento Preventivo" },
                    new TipoServicio { NombreTipoServ = "Reparación Correctiva" },
                    new TipoServicio { NombreTipoServ = "Diagnóstico" },
                    new TipoServicio { NombreTipoServ = "Revisión Técnica" }
                );
                results.Add("Tipos de servicio creados");
            }
            
            if (!_db.EstadosServicio.Any())
            {
                _db.EstadosServicio.AddRange(
                    new EstadoServ { NombreEstServ = "Pendiente" },
                    new EstadoServ { NombreEstServ = "En Proceso" },
                    new EstadoServ { NombreEstServ = "Completada" },
                    new EstadoServ { NombreEstServ = "Cancelada" }
                );
                results.Add("Estados de servicio creados");
            }
            
            if (!_db.TiposPago.Any())
            {
                _db.TiposPago.AddRange(
                    new TipoPago { NombreTipoPag = "Efectivo" },
                    new TipoPago { NombreTipoPag = "Tarjeta Débito" },
                    new TipoPago { NombreTipoPag = "Tarjeta Crédito" },
                    new TipoPago { NombreTipoPag = "Transferencia" }
                );
                results.Add("Tipos de pago creados");
            }

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

            _logger.LogInformation("Datos básicos del sistema inicializados");

            return Ok(new { 
                message = "Datos básicos inicializados exitosamente",
                operations = results,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al inicializar datos básicos del sistema");
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
                return NotFound("Usuario no encontrado");

            return Ok(new UsuarioDto
            {
                Id = usuario.Id,
                Email = usuario.Email ?? string.Empty,
                RolNombre = usuario.UserMemberRoles?.FirstOrDefault()?.Rol?.NombreRol ?? string.Empty,
                EstadoNombre = "Activo"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuario {UserId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Actualizar información de usuario
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="request">Datos actualizados</param>
    /// <returns>Usuario actualizado</returns>
    [HttpPut("users/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UsuarioDto>> UpdateUser(int id, [FromBody] UpdateUsuarioDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var usuario = await _unitOfWork.UserMembers.GetByIdAsync(id);
            if (usuario == null)
                return NotFound("Usuario no encontrado");

            // Verifica email único si cambió
            if (usuario.Email != request.Email)
            {
                var existingUser = await _unitOfWork.UserMembers.GetByUserNameAsync(request.Email);
                if (existingUser != null)
                    return BadRequest("El email ya está en uso");
            }

            usuario.Email = request.Email;
            usuario.Username = request.Email; // Mantiene sincronizado

            await _unitOfWork.UserMembers.UpdateAsync(usuario);

            _logger.LogInformation("Usuario actualizado: {Email}", usuario.Email);

            return Ok(new UsuarioDto
            {
                Id = usuario.Id,
                Email = usuario.Email ?? string.Empty,
                RolNombre = usuario.UserMemberRoles?.FirstOrDefault()?.Rol?.NombreRol ?? string.Empty,
                EstadoNombre = "Activo"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar usuario {UserId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Cambiar contraseña de usuario
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="request">Datos de cambio de contraseña</param>
    /// <returns>Confirmación de cambio</returns>
    [HttpPut("users/{id}/change-password")]
    [Authorize]
    public async Task<ActionResult> ChangeUserPassword(int id, [FromBody] ChangePasswordDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var usuario = await _unitOfWork.UserMembers.GetByIdAsync(id);
            if (usuario == null)
                return NotFound("Usuario no encontrado");

            // Cambia la contraseña usando el password hasher
            usuario.Password = _passwordHasher.HashPassword(usuario, request.NewPassword);

            await _unitOfWork.UserMembers.UpdateAsync(usuario);

            _logger.LogInformation("Contraseña cambiada para usuario ID: {UserId}", id);
            
            return Ok("Contraseña actualizada exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar contraseña del usuario {UserId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Asignar rol a usuario
    /// </summary>
    /// <param name="model">Datos de asignación de rol</param>
    /// <returns>Confirmación de asignación</returns>
    [HttpPost("users/assign-role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignRoleToUser([FromBody] AddRoleDto model)
    {
        try
        {
            var result = await _userService.AddRoleAsync(model);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al asignar rol al usuario");
            return StatusCode(500, "Error interno del servidor");
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
            var roles = await _unitOfWork.Roles.GetAllAsync();
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
            _logger.LogError(ex, "Error al obtener roles");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    #endregion

    #region Métodos Privados

    private void SetRefreshTokenInCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(10),
            Secure = true,
            SameSite = SameSiteMode.Strict
        };
        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }

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
            return _db.TiposCliente.Any() && 
                   _db.TiposVehiculo.Any() && 
                   _db.Roles.Any();
        }
        catch
        {
            return false;
        }
    }

    #endregion
}
