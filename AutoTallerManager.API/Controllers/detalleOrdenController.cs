using System;
using Microsoft.AspNetCore.RateLimiting;
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

        // Crear detalle: consolidado en OrdenesServicioController

        // Update detalle: consolidado en OrdenesServicioController

        // Delete detalle: consolidado en OrdenesServicioController
    }
}