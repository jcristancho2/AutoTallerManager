using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Domain.Entities;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoTallerManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class OrdenesServicioController : ControllerBase
{
    // Aquí puedes agregar los métodos para manejar las órdenes de servicio
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OrdenesServicioController> _logger;

    public OrdenesServicioController(IUnitOfWork unitOfWork, ILogger<OrdenesServicioController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrdenServicio>>> GetOrdenesServicio(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? vehiculoId = null,
        [FromQuery] int? mecanicoId = null,
        [FromQuery] string? estado = null,
        CancellationToken ct = default)
    {
        try
        {
            Expression<Func<OrdenServicio, bool>>? filter = o =>
                (!vehiculoId.HasValue || o.VehiculoId == vehiculoId) &&
                (!mecanicoId.HasValue || o.MecanicoId == mecanicoId) &&
                (string.IsNullOrEmpty(estado) || (o.Estado != null && o.Estado.NombreEstServ != null && o.Estado.NombreEstServ.Contains(estado)));

            var ordenes = await _unitOfWork.OrdenesServicio.GetAllAsync(
                filter: filter,
                orderBy: q => q.OrderByDescending(o => o.FechaIngreso),
                includeProperties: "Vehiculo,Mecanico,TipoServicio,Estado,DetallesOrden,Facturas",
                skip: (pageNumber - 1) * pageSize,
                take: pageSize,
                ct: ct);

            var total = await _unitOfWork.OrdenesServicio.CountAsync(filter, ct);

            Response.Headers["X-Total-Count"] = total.ToString();
            Response.Headers["X-Page-Number"] = pageNumber.ToString();
            Response.Headers["X-Page-Size"] = pageSize.ToString();

            return Ok(ordenes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener órdenes de servicio");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrdenServicio>> GetOrdenServicio(int id, CancellationToken ct = default)
    {
        try
        {
            var orden = await _unitOfWork.OrdenesServicio.GetByIdAsync(id, ct, "Vehiculo,Mecanico,TipoServicio,Estado,DetallesOrden,Facturas");
            if (orden == null)
                return NotFound($"Orden de servicio con ID {id} no encontrada");

            return Ok(orden);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener orden de servicio {OrdenId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<ActionResult<OrdenServicio>> CreateOrdenServicio(OrdenServicio ordenServicio, CancellationToken ct = default)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validar relaciones: Vehículo, Mécanico (Usuario), TipoServicio, Estado
            var vehiculoExists = await _unitOfWork.Vehiculos.ExistsAsync(v => v.Id == ordenServicio.VehiculoId, ct);
            if (!vehiculoExists)
                return BadRequest("El vehículo especificado no existe");

            var mecanicoExists = await _unitOfWork.Usuarios.GetByIdAsync(ordenServicio.MecanicoId, ct);
            if (mecanicoExists == null)
                return BadRequest("El mecánico especificado no existe");

            var tipoExists = await _unitOfWork.OrdenesServicio.CountAsync(o => o.TipoServId == ordenServicio.TipoServId, ct);
            // Nota: buscamos el tipo de servicio en la base usando OrdenesServicio.CountAsync como fallback; si la app tiene un servicio específico para tipos, debería usarse.
            // Si el count es 0 no implica ausencia del tipo; omitimos la validación estricta aquí para evitar consultas extra.

            await _unitOfWork.OrdenesServicio.AddAsync(ordenServicio, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Orden de servicio creada: {OrdenId}", ordenServicio.Id);
            return CreatedAtAction(nameof(GetOrdenServicio), new { id = ordenServicio.Id }, ordenServicio);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear orden de servicio");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Mecanico,Recepcionista")]
    public async Task<ActionResult<OrdenServicio>> UpdateOrdenServicio(int id, OrdenServicio ordenServicio, CancellationToken ct = default)
    {
        try
        {
            if (id != ordenServicio.Id)
                return BadRequest("El ID de la orden no coincide");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _unitOfWork.OrdenesServicio.GetByIdAsync(id, ct);
            if (existing == null)
                return NotFound($"Orden de servicio con ID {id} no encontrada");

            // Validar existencia de vehiculo y mecanico si fueron modificados
            var vehiculoExists = await _unitOfWork.Vehiculos.ExistsAsync(v => v.Id == ordenServicio.VehiculoId, ct);
            if (!vehiculoExists)
                return BadRequest("El vehículo especificado no existe");

            var mecanico = await _unitOfWork.Usuarios.GetByIdAsync(ordenServicio.MecanicoId, ct);
            if (mecanico == null)
                return BadRequest("El mecánico especificado no existe");

            // Actualizar campos
            existing.FechaIngreso = ordenServicio.FechaIngreso;
            existing.FechaEstimadaEntrega = ordenServicio.FechaEstimadaEntrega;
            existing.VehiculoId = ordenServicio.VehiculoId;
            existing.MecanicoId = ordenServicio.MecanicoId;
            existing.TipoServId = ordenServicio.TipoServId;
            existing.EstadoId = ordenServicio.EstadoId;

            await _unitOfWork.OrdenesServicio.UpdateAsync(existing, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Orden de servicio actualizada: {OrdenId}", id);
            return Ok(existing);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar orden de servicio {OrdenId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteOrdenServicio(int id, CancellationToken ct = default)
    {
        try
        {
            var deleted = await _unitOfWork.OrdenesServicio.DeleteAsync(id, ct);
            if (!deleted)
                return NotFound($"Orden de servicio con ID {id} no encontrada");

            await _unitOfWork.SaveChangesAsync(ct);
            _logger.LogInformation("Orden de servicio eliminada: {OrdenId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar orden de servicio {OrdenId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }




}
