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
    public class DetailOrderController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DetailOrderController> _logger;

        public DetailOrderController(IUnitOfWork unitOfWork, ILogger<DetailOrderController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DetailOrder>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] int? serviceOrderId = null,
            [FromQuery] int? partId = null,
            CancellationToken ct = default)
        {
            try
            {
                var filter = new Func<DetailOrder, bool>(d =>
                    (!serviceOrderId.HasValue || d.ServiceOrderId == serviceOrderId) &&
                    (!sparepartId.HasValue || (d.SparePartId.HasValue && d.SparePartId.Value == sparepartId)));

                // Convertir a expresión sencilla usando GetAllAsync con string include
                var details = await _unitOfWork.DetallesOrden.GetAllAsync(
                    filter: d => (!serviceOrderId.HasValue || d.ServiceOrderId == serviceOrderId) &&
                                (!sparepartId.HasValue || (d.SparePartId.HasValue && d.SparePartId.Value == sparepartId)),
                    orderBy: q => q.OrderBy(d => d.DetailOrderId),
                    includeProperties: "SparePart,ServiceOrder",
                    skip: (pageNumber - 1) * pageSize,
                    take: pageSize,
                    ct: ct);

                var total = await _unitOfWork.DetallesOrden.CountAsync(
                    d => (!serviceOrderId.HasValue || d.ServiceOrderId == serviceOrderId) &&
                        (!sparepartId.HasValue || (d.SparePartId.HasValue && d.SparePartId.Value == sparepartId)),
                    ct);

                Response.Headers["X-Total-Count"] = total.ToString();
                Response.Headers["X-Page-Number"] = pageNumber.ToString();
                Response.Headers["X-Page-Size"] = pageSize.ToString();

                return Ok(details);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalles de orden");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("{orderId}/details/{detailId}")]
        public async Task<ActionResult<DetailOrder>> GetById(int orderId, int detailId, CancellationToken ct = default)
        {
            try
            {
                var detail = await _unitOfWork.DetailOrder.GetByIdAsync(detailId, orderId, ct);
                if (detail == null) return NotFound("Detalle no encontrado");
                return Ok(detail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalle {DetailId} de orden {OrderId}", detailId, orderId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // Crear detalle: consolidado en OrdenesServicioController

        // Update detalle: consolidado en OrdenesServicioController

        // Delete detalle: consolidado en OrdenesServicioController
    }
}