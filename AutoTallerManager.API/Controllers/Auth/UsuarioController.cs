 using System;
using AutoTallerManager.API.DTOs.Auth;
using AutoTallerManager.Application.Abstractions;
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


    [HttpPost("register")]
    // [Authorize(Roles = "Admin")] // Temporalmente comentado para crear el primer admin
    public async Task<ActionResult<UsuarioDto>> Register([FromBody] CreateUsuarioDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Usar servicio de registro mejorado
        var registerDto = new RegisterDto
        {
            Username = request.Email,
            Email = request.Email,
            Password = request.Password
            // UserMember maneja roles automáticamente
        };

        var result = await _userService.RegisterAsync(registerDto);
        
        // Obtiene usuario creado para respuesta detallada
        var usuario = await _unitOfWork.UserMembers.GetByUserNameAsync(request.Email);
        if (usuario == null)
            return BadRequest("Usuario creado pero no encontrado");

        var rol = usuario.UserMemberRoles?.FirstOrDefault()?.Rol;

        _logger.LogInformation("Usuario creado exitosamente: {Email}", usuario.Email);

        return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, new UsuarioDto
        {
            Id = usuario.Id,
            Email = usuario.Email ?? string.Empty,
            RolNombre = rol?.NombreRol ?? string.Empty,
            EstadoNombre = "Activo"
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
        return Ok(result);
    }

    #endregion

    #region Gestión de Usuarios (CRUD Completo)

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetAll()
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

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<UsuarioDto>> GetById(int id)
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

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UsuarioDto>> Update(int id, [FromBody] UpdateUsuarioDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

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

    [HttpPut("{id}/cambiar-password")]
    [Authorize]
    public async Task<ActionResult> ChangePassword(int id, [FromBody] ChangePasswordDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var usuario = await _unitOfWork.UserMembers.GetByIdAsync(id);
        if (usuario == null)
            return NotFound("Usuario no encontrado");

        // Cambia la contraseña usando el password hasher
        var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<UserMember>();
        usuario.Password = passwordHasher.HashPassword(usuario, request.NewPassword);

        await _unitOfWork.UserMembers.UpdateAsync(usuario);

        _logger.LogInformation("Contraseña cambiada para usuario ID: {UserId}", id);
        
        return Ok("Contraseña actualizada exitosamente");
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

    #endregion
}