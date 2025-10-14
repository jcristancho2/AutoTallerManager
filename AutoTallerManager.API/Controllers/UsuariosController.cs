using System;
using Microsoft.AspNetCore.RateLimiting;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Abstractions.Auth;
using AutoTallerManager.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoTallerManager.API.Controllers
{/*
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(IUnitOfWork unitOfWork, ILogger<UsuariosController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null, CancellationToken ct = default)
        {
            try
            {
                // El servicio expone GetAllAsync con filtros expresables
                var usuarios = await _unitOfWork.Usuarios.GetAllAsync(
                    filter: string.IsNullOrEmpty(search) ? null : (System.Linq.Expressions.Expression<Func<Usuario, bool>>)(u => u.Email != null && u.Email.Contains(search)),
                    orderBy: q => q.OrderBy(u => u.Email),
                    includeProperties: "Rol,EstadoUsuario",
                    ct: ct);

                // Aplicar paginación en memoria si el servicio no la soporta directamente
                var paged = usuarios.Skip((pageNumber - 1) * pageSize).Take(pageSize);

                Response.Headers["X-Page-Number"] = pageNumber.ToString();
                Response.Headers["X-Page-Size"] = pageSize.ToString();

                return Ok(paged);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetById(int id, CancellationToken ct = default)
        {
            try
            {
                var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id, ct);
                if (usuario == null) return NotFound($"Usuario con ID {id} no encontrado");

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario {UsuarioId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Usuario>> Create([FromBody] Usuario usuario, CancellationToken ct = default)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (string.IsNullOrEmpty(usuario.Email))
                    return BadRequest("El email es obligatorio");

                var exists = await _unitOfWork.Usuarios.GetByEmailAsync(usuario.Email!, ct);
                if (exists != null) return BadRequest("Ya existe un usuario con este email");

                var created = await _unitOfWork.Usuarios.CreateAsync(usuario, ct);
                _logger.LogInformation("Usuario creado: {UsuarioId}", created.Id);

                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Usuario>> Update(int id, [FromBody] Usuario usuario, CancellationToken ct = default)
        {
            try
            {
                if (id != usuario.Id) return BadRequest("El ID del usuario no coincide");
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var existing = await _unitOfWork.Usuarios.GetByIdAsync(id, ct);
                if (existing == null) return NotFound($"Usuario con ID {id} no encontrado");

                if (!string.Equals(existing.Email, usuario.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var byEmail = await _unitOfWork.Usuarios.GetByEmailAsync(usuario.Email ?? string.Empty, ct);
                    if (byEmail != null && byEmail.Id != id) return BadRequest("El email ya está en uso por otro usuario");
                }

                // Actualizar campos permitidos
                existing.Email = usuario.Email;
                existing.RolId = usuario.RolId;
                existing.EstadoUsuarioId = usuario.EstadoUsuarioId;

                var updated = await _unitOfWork.Usuarios.UpdateAsync(existing, ct);
                _logger.LogInformation("Usuario actualizado: {UsuarioId}", id);

                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario {UsuarioId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("{id}/cambiar-password")]
        public async Task<ActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request, CancellationToken ct = default)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.NewPassword))
                    return BadRequest("Nueva contraseña requerida");

                var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id, ct);
                if (usuario == null) return NotFound("Usuario no encontrado");

                var result = await _unitOfWork.Usuarios.ChangePasswordAsync(id, request.NewPassword, ct);
                if (!result) return BadRequest("No se pudo cambiar la contraseña");

                _logger.LogInformation("Contraseña actualizada para usuario {UsuarioId}", id);
                return Ok("Contraseña actualizada");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar contraseña del usuario {UsuarioId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("{id}/activar")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Activate(int id, CancellationToken ct = default)
        {
            try
            {
                var ok = await _unitOfWork.Usuarios.ActivateUserAsync(id, ct);
                if (!ok) return BadRequest("No se pudo activar el usuario");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al activar usuario {UsuarioId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("{id}/desactivar")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Deactivate(int id, CancellationToken ct = default)
        {
            try
            {
                var ok = await _unitOfWork.Usuarios.DeactivateUserAsync(id, ct);
                if (!ok) return BadRequest("No se pudo desactivar el usuario");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desactivar usuario {UsuarioId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        public class ChangePasswordRequest
        {
            public string? NewPassword { get; set; }
        }   
    */
    } 
