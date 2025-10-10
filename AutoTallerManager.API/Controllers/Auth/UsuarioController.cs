using AutoTallerManager.API.DTOs.Auth;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Domain.Entities.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoTallerManager.API.Services;

namespace AutoTallerManager.API.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UsuarioController> _logger;
    private readonly IJwtService _jwtService;

    public UsuarioController(IUnitOfWork unitOfWork, ILogger<UsuarioController> logger, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<UsuarioLoginResponseDto>> Login([FromBody] LoginUsuarioDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var isValid = await _unitOfWork.Usuarios.ValidateCredentialsAsync(request.Email, request.Password);
        
        if (!isValid)
            return Unauthorized("Credenciales inválidas");

        var usuario = await _unitOfWork.Usuarios.GetByEmailAsync(request.Email);
        
        if (usuario == null)
            return Unauthorized("Usuario no encontrado");

        // Generar JWT token real
        var token = _jwtService.GenerateToken(usuario);

        return Ok(new UsuarioLoginResponseDto
        {
            Id = usuario.Id,
            Email = usuario.Email ?? string.Empty,                           // ✅ CORREGIR
            RolNombre = usuario.Rol?.NombreRol ?? string.Empty,              // ✅ CORREGIR
            EstadoNombre = usuario.EstadoUsuario?.NombreEstUsu ?? string.Empty, // ✅ CORREGIR
            Token = token
        });
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UsuarioDto>> Register([FromBody] CreateUsuarioDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Verificar si el email ya existe
        var existingUser = await _unitOfWork.Usuarios.GetByEmailAsync(request.Email);
        if (existingUser != null)
            return BadRequest("El email ya está registrado");

        // Verificar que el rol existe
        var rol = await _unitOfWork.Roles.GetByIdAsync(request.RolId);
        if (rol == null)
            return BadRequest("Rol no válido");

        // Verificar que el estado existe
        var estado = await _unitOfWork.EstadosUsuario.GetByIdAsync(request.EstadoUsuarioId);
        if (estado == null)
            return BadRequest("Estado de usuario no válido");

        var usuario = new Usuario
        {
            Email = request.Email,
            PasswordHash = request.Password, // En producción usar BCrypt
            RolId = request.RolId,
            EstadoUsuarioId = request.EstadoUsuarioId
        };

        await _unitOfWork.Usuarios.CreateAsync(usuario);

        _logger.LogInformation("Usuario creado exitosamente: {Email}", usuario.Email);

        return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, new UsuarioDto
        {
            Id = usuario.Id,
            Email = usuario.Email ?? string.Empty,              // ✅ CORREGIR
            RolNombre = rol.NombreRol ?? string.Empty,          // ✅ CORREGIR
            EstadoNombre = estado.NombreEstUsu ?? string.Empty  // ✅ CORREGIR
        });
    }

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
            Email = u.Email ?? string.Empty,                        // ✅ CORREGIR
            RolNombre = u.Rol?.NombreRol ?? string.Empty,           // ✅ CORREGIR
            EstadoNombre = u.EstadoUsuario?.NombreEstUsu ?? string.Empty // ✅ CORREGIR
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
            Email = usuario.Email ?? string.Empty,                      // ✅ CORREGIR
            RolNombre = usuario.Rol?.NombreRol ?? string.Empty,         // ✅ CORREGIR
            EstadoNombre = usuario.EstadoUsuario?.NombreEstUsu ?? string.Empty // ✅ CORREGIR
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
            Email = usuario.Email ?? string.Empty,              // ✅ CORREGIR
            RolNombre = rol.NombreRol ?? string.Empty,          // ✅ CORREGIR
            EstadoNombre = estado.NombreEstUsu ?? string.Empty  // ✅ CORREGIR
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

    [HttpGet("roles")]
    public async Task<ActionResult<IEnumerable<RolDto>>> GetRoles()
    {
        var roles = await _unitOfWork.Roles.GetAllAsync();
        var rolesDto = roles.Select(r => new RolDto
        {
            Id = r.Id,
            NombreRol = r.NombreRol ?? string.Empty,    // ✅ CORREGIR
            Descripcion = r.Descripcion ?? string.Empty // ✅ CORREGIR
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
            NombreEstUsu = e.NombreEstUsu ?? string.Empty // ✅ CORREGIR
        });

        return Ok(estadosDto);
    }

}