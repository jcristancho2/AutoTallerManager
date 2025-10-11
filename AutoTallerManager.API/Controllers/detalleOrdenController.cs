using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoTallerManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DetalleOrdenController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DetalleOrdenController> _logger;

        public DetalleOrdenController(IUnitOfWork unitOfWork, ILogger<DetalleOrdenController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DetalleOrden>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] int? ordenServicioId = null,
            [FromQuery] int? repuestoId = null,
            CancellationToken ct = default)
        {
            try
            {
                var filter = new Func<DetalleOrden, bool>(d =>
                    (!ordenServicioId.HasValue || d.OrdenServicioId == ordenServicioId) &&
                    (!repuestoId.HasValue || (d.RepuestoId.HasValue && d.RepuestoId.Value == repuestoId)));

                // Convertir a expresión sencilla usando GetAllAsync con string include
                var detalles = await _unitOfWork.DetallesOrden.GetAllAsync(
                    filter: d => (!ordenServicioId.HasValue || d.OrdenServicioId == ordenServicioId) &&
                                 (!repuestoId.HasValue || (d.RepuestoId.HasValue && d.RepuestoId.Value == repuestoId)),
                    orderBy: q => q.OrderBy(d => d.DetalleOrdenId),
                    includeProperties: "Repuesto,OrdenServicio",
                    skip: (pageNumber - 1) * pageSize,
                    take: pageSize,
                    ct: ct);

                var total = await _unitOfWork.DetallesOrden.CountAsync(
                    d => (!ordenServicioId.HasValue || d.OrdenServicioId == ordenServicioId) &&
                         (!repuestoId.HasValue || (d.RepuestoId.HasValue && d.RepuestoId.Value == repuestoId)),
                    ct);

                Response.Headers["X-Total-Count"] = total.ToString();
                Response.Headers["X-Page-Number"] = pageNumber.ToString();
                Response.Headers["X-Page-Size"] = pageSize.ToString();

                return Ok(detalles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalles de orden");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("{ordenId}/detalles/{detalleId}")]
        public async Task<ActionResult<DetalleOrden>> GetById(int ordenId, int detalleId, CancellationToken ct = default)
        {
            try
            {
                var detalle = await _unitOfWork.DetallesOrden.GetByIdAsync(detalleId, ordenId, ct);
                if (detalle == null) return NotFound("Detalle no encontrado");
                return Ok(detalle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalle {DetalleId} de orden {OrdenId}", detalleId, ordenId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost("{ordenId}/detalles")]
        [Authorize(Roles = "Admin,Mecanico")]
        public async Task<ActionResult<DetalleOrden>> Create(int ordenId, [FromBody] DetalleOrden detalle, CancellationToken ct = default)
        {
            try
            {
                var orden = await _unitOfWork.OrdenesServicio.GetByIdAsync(ordenId, ct);
                if (orden == null) return NotFound("Orden de servicio no encontrada");

                detalle.OrdenServicioId = ordenId;

                if (detalle.RepuestoId.HasValue && detalle.RepuestoId.Value > 0)
                {
                    var repuesto = await _unitOfWork.Repuestos.GetByIdAsync(detalle.RepuestoId.Value, ct);
                    if (repuesto == null) return BadRequest("Repuesto no existe");
                    if (detalle.Cantidad <= 0) return BadRequest("Cantidad debe ser mayor a 0");
                    if (repuesto.Stock < detalle.Cantidad) return BadRequest("Stock insuficiente del repuesto");

                    repuesto.Stock -= detalle.Cantidad;
                    await _unitOfWork.Repuestos.UpdateAsync(repuesto, ct);
                }

                await _unitOfWork.DetallesOrden.AddAsync(detalle, ct);
                await _unitOfWork.SaveChangesAsync(ct);

                _logger.LogInformation("Detalle agregado a orden {OrdenId}: DetalleId {DetalleOrdenId}", ordenId, detalle.DetalleOrdenId);
                return CreatedAtAction(nameof(GetById), new { ordenId = ordenId, detalleId = detalle.DetalleOrdenId }, detalle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear detalle para orden {OrdenId}", ordenId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("{ordenId}/detalles/{detalleId}")]
        [Authorize(Roles = "Admin,Mecanico")]
        public async Task<ActionResult<DetalleOrden>> Update(int ordenId, int detalleId, [FromBody] DetalleOrden detalle, CancellationToken ct = default)
        {
            try
            {
                var existente = await _unitOfWork.DetallesOrden.GetByIdAsync(detalleId, ordenId, ct);
                if (existente == null) return NotFound("Detalle no encontrado");

                // Manejo de stock similar al controller de ordenes
                if (existente.RepuestoId != detalle.RepuestoId)
                {
                    if (existente.RepuestoId.HasValue)
                    {
                        var repuestoAnterior = await _unitOfWork.Repuestos.GetByIdAsync(existente.RepuestoId.Value, ct);
                        if (repuestoAnterior != null)
                        {
                            repuestoAnterior.Stock += existente.Cantidad;
                            await _unitOfWork.Repuestos.UpdateAsync(repuestoAnterior, ct);
                        }
                    }

                    if (detalle.RepuestoId.HasValue)
                    {
                        var repuestoNuevo = await _unitOfWork.Repuestos.GetByIdAsync(detalle.RepuestoId.Value, ct);
                        if (repuestoNuevo == null) return BadRequest("Repuesto nuevo no existe");
                        if (repuestoNuevo.Stock < detalle.Cantidad) return BadRequest("Stock insuficiente del nuevo repuesto");
                        repuestoNuevo.Stock -= detalle.Cantidad;
                        await _unitOfWork.Repuestos.UpdateAsync(repuestoNuevo, ct);
                    }
                }
                else if (existente.RepuestoId.HasValue && detalle.RepuestoId.HasValue && existente.Cantidad != detalle.Cantidad)
                {
                    var repuesto = await _unitOfWork.Repuestos.GetByIdAsync(detalle.RepuestoId.Value, ct);
                    if (repuesto == null) return BadRequest("Repuesto no existe");

                    var diferencia = detalle.Cantidad - existente.Cantidad;
                    if (diferencia > 0 && repuesto.Stock < diferencia) return BadRequest("Stock insuficiente del repuesto");
                    repuesto.Stock -= diferencia;
                    await _unitOfWork.Repuestos.UpdateAsync(repuesto, ct);
                }

                existente.RepuestoId = detalle.RepuestoId;
                existente.Descripcion = detalle.Descripcion;
                existente.Cantidad = detalle.Cantidad;
                existente.PrecioUnitario = detalle.PrecioUnitario;
                existente.PrecioManoDeObra = detalle.PrecioManoDeObra;

                await _unitOfWork.DetallesOrden.UpdateAsync(existente, ct);
                await _unitOfWork.SaveChangesAsync(ct);

                return Ok(existente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar detalle {DetalleId} de orden {OrdenId}", detalleId, ordenId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpDelete("{ordenId}/detalles/{detalleId}")]
        [Authorize(Roles = "Admin,Mecanico")]
        public async Task<ActionResult> Delete(int ordenId, int detalleId, CancellationToken ct = default)
        {
            try
            {
                var existente = await _unitOfWork.DetallesOrden.GetByIdAsync(detalleId, ordenId, ct);
                if (existente == null) return NotFound("Detalle no encontrado");

                if (existente.RepuestoId.HasValue)
                {
                    var repuesto = await _unitOfWork.Repuestos.GetByIdAsync(existente.RepuestoId.Value, ct);
                    if (repuesto != null)
                    {
                        repuesto.Stock += existente.Cantidad;
                        await _unitOfWork.Repuestos.UpdateAsync(repuesto, ct);
                    }
                }

                var deleted = await _unitOfWork.DetallesOrden.DeleteAsync(detalleId, ordenId, ct);
                if (!deleted) return NotFound();

                await _unitOfWork.SaveChangesAsync(ct);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar detalle {DetalleId} de orden {OrdenId}", detalleId, ordenId);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}