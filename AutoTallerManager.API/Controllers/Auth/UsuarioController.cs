using System;
using AutoTallerManager.API.DTOs.Auth;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Domain.Entities.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoTallerManager.API.Services;
using AutoTallerManager.API.Services.Interfaces.Auth;

namespace AutoTallerManager.API.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UsuarioController> _logger;
    private readonly IJwtService _jwtService;
    private readonly IUserService _userService;

    public UsuarioController(
        IUnitOfWork unitOfWork, 
        ILogger<UsuarioController> logger, 
        IJwtService jwtService,
        IUserService userService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _jwtService = jwtService;
        _userService = userService;
    }

    #region Autenticación y Tokens (Mejorado con Refresh Tokens)
    
    [HttpPost("login")]
    public async Task<ActionResult<UsuarioLoginResponseDto>> Login([FromBody] LoginUsuarioDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Usar el servicio de autenticación mejorado
        var loginDto = new LoginDto 
        { 
            Email = request.Email, 
            Password = request.Password 
        };
        
        var tokenResult = await _userService.GetTokenAsync(loginDto);
        
        if (tokenResult == null || string.IsNullOrEmpty(tokenResult.Token))
            return Unauthorized("Credenciales inválidas");

        // Establecer refresh token en cookie
        if (!string.IsNullOrEmpty(tokenResult.RefreshToken))
        {
            SetRefreshTokenInCookie(tokenResult.RefreshToken);
        }

        // Obtener información del usuario para la respuesta
        var usuario = await _unitOfWork.Usuarios.GetByEmailAsync(request.Email);
        if (usuario == null)
            return Unauthorized("Usuario no encontrado");

        return Ok(new UsuarioLoginResponseDto
        {
            Id = usuario.Id,
            Email = usuario.Email ?? string.Empty,
            RolNombre = usuario.Rol?.NombreRol ?? string.Empty,
            EstadoNombre = usuario.EstadoUsuario?.NombreEstUsu ?? string.Empty,
            Token = tokenResult.Token,
            RefreshToken = tokenResult.RefreshToken
        });
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UsuarioDto>> Register([FromBody] CreateUsuarioDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Usar servicio de registro mejorado
        var registerDto = new RegisterDto
        {
            Email = request.Email,
            Password = request.Password,
            RolId = request.RolId,
            EstadoUsuarioId = request.EstadoUsuarioId
        };

        var result = await _userService.RegisterAsync(registerDto);
        
        if (!result.IsSuccess)
            return BadRequest(result.Message);

        // Obtener usuario creado para respuesta detallada
        var usuario = await _unitOfWork.Usuarios.GetByEmailAsync(request.Email);
        if (usuario == null)
            return BadRequest("Usuario creado pero no encontrado");

        var rol = await _unitOfWork.Roles.GetByIdAsync(request.RolId);
        var estado = await _unitOfWork.EstadosUsuario.GetByIdAsync(request.EstadoUsuarioId);

        _logger.LogInformation("Usuario creado exitosamente: {Email}", usuario.Email);

        return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, new UsuarioDto
        {
            Id = usuario.Id,
            Email = usuario.Email ?? string.Empty,
            RolNombre = rol?.NombreRol ?? string.Empty,
            EstadoNombre = estado?.NombreEstUsu ?? string.Empty
        });
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
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

        return Ok(response);
    }

    [HttpPost("addrole")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddRoleAsync([FromBody] AddRoleDto model)
    {
        var result = await _userService.AddRoleAsync(model);
        
        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result);
    }

    #endregion

    #region Gestión de Usuarios (CRUD Completo)

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetAll()
    {
        var usuarios = await _unitOfWork.Usuarios.GetAllAsync(
            includeProperties: "Rol,EstadoUsuario"
        );

        var usuariosDto = usuarios.Select(u => new UsuarioDto
        {
            Id = u.Id,
            Email = u.Email ?? string.Empty,
            RolNombre = u.Rol?.NombreRol ?? string.Empty,
            EstadoNombre = u.EstadoUsuario?.NombreEstUsu ?? string.Empty
        });

        return Ok(usuariosDto);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<UsuarioDto>> GetById(int id)
    {
        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id);
        
        if (usuario == null)
            return NotFound("Usuario no encontrado");

        return Ok(new UsuarioDto
        {
            Id = usuario.Id,
            Email = usuario.Email ?? string.Empty,
            RolNombre = usuario.Rol?.NombreRol ?? string.Empty,
            EstadoNombre = usuario.EstadoUsuario?.NombreEstUsu ?? string.Empty
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UsuarioDto>> Update(int id, [FromBody] UpdateUsuarioDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id);
        if (usuario == null)
            return NotFound("Usuario no encontrado");

        // Verificar email único si cambió
        if (usuario.Email != request.Email)
        {
            var existingUser = await _unitOfWork.Usuarios.GetByEmailAsync(request.Email);
            if (existingUser != null)
                return BadRequest("El email ya está en uso");
        }

        // Verificar rol válido
        var rol = await _unitOfWork.Roles.GetByIdAsync(request.RolId);
        if (rol == null)
            return BadRequest("Rol no válido");

        // Verificar estado válido
        var estado = await _unitOfWork.EstadosUsuario.GetByIdAsync(request.EstadoUsuarioId);
        if (estado == null)
            return BadRequest("Estado no válido");

        usuario.Email = request.Email;
        usuario.RolId = request.RolId;
        usuario.EstadoUsuarioId = request.EstadoUsuarioId;

        await _unitOfWork.Usuarios.UpdateAsync(usuario);

        _logger.LogInformation("Usuario actualizado: {Email}", usuario.Email);

        return Ok(new UsuarioDto
        {
            Id = usuario.Id,
            Email = usuario.Email ?? string.Empty,
            RolNombre = rol.NombreRol ?? string.Empty,
            EstadoNombre = estado.NombreEstUsu ?? string.Empty
        });
    }

    [HttpPut("{id}/cambiar-password")]
    [Authorize]
    public async Task<ActionResult> ChangePassword(int id, [FromBody] ChangePasswordDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var success = await _unitOfWork.Usuarios.ChangePasswordAsync(id, request.NewPassword);
        
        if (!success)
            return NotFound("Usuario no encontrado");

        _logger.LogInformation("Contraseña cambiada para usuario ID: {UserId}", id);
        
        return Ok("Contraseña actualizada exitosamente");
    }

    [HttpPut("{id}/activar")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> ActivateUser(int id)
    {
        var success = await _unitOfWork.Usuarios.ActivateUserAsync(id);
        
        if (!success)
            return NotFound("Usuario no encontrado o estado 'Activo' no existe");

        _logger.LogInformation("Usuario activado ID: {UserId}", id);
        
        return Ok("Usuario activado exitosamente");
    }

    [HttpPut("{id}/desactivar")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeactivateUser(int id)
    {
        var success = await _unitOfWork.Usuarios.DeactivateUserAsync(id);
        
        if (!success)
            return NotFound("Usuario no encontrado o estado 'NoActivo' no existe");

        _logger.LogInformation("Usuario desactivado ID: {UserId}", id);
        
        return Ok("Usuario desactivado exitosamente");
    }

    #endregion

    #region Catálogos y Utilidades

    [HttpGet("roles")]
    public async Task<ActionResult<IEnumerable<RolDto>>> GetRoles()
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

    [HttpGet("estados")]
    public async Task<ActionResult<IEnumerable<EstadoUsuarioDto>>> GetEstados()
    {
        var estados = await _unitOfWork.EstadosUsuario.GetAllAsync();
        var estadosDto = estados.Select(e => new EstadoUsuarioDto
        {
            Id = e.Id,
            NombreEstUsu = e.NombreEstUsu ?? string.Empty
        });

        return Ok(estadosDto);
    }

    #endregion

    #region Métodos Privados

    private void SetRefreshTokenInCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(10),
            Secure = true, // Solo en HTTPS en producción
            SameSite = SameSiteMode.Strict
        };
        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }

    #endregion
}