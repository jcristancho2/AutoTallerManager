using System;
using Microsoft.AspNetCore.RateLimiting;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoTallerManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[EnableRateLimiting("Facturas")]
public class FacturasController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FacturasController> _logger;

    public FacturasController(IUnitOfWork unitOfWork, ILogger<FacturasController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Factura>>> GetFacturas(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? clienteId = null,
        [FromQuery] int? ordenServicioId = null,
        [FromQuery] DateTime? fechaDesde = null,
        [FromQuery] DateTime? fechaHasta = null,
        CancellationToken ct = default)
    {
        try
        {
            Expression<Func<Factura, bool>>? filter = f =>
                (!clienteId.HasValue || f.ClienteId == clienteId) &&
                (!ordenServicioId.HasValue || f.OrdenServicioId == ordenServicioId) &&
                (!fechaDesde.HasValue || f.Fecha >= fechaDesde.Value) &&
                (!fechaHasta.HasValue || f.Fecha <= fechaHasta.Value);

            var facturas = await _unitOfWork.Facturas.GetAllAsync(
                filter: filter,
                orderBy: q => q.OrderByDescending(f => f.Fecha),
                includeProperties: "Cliente,OrdenServicio,TipoPago",
                skip: (pageNumber - 1) * pageSize,
                take: pageSize,
                ct: ct);

            var total = await _unitOfWork.Facturas.CountAsync(filter, ct);
            Response.Headers["X-Total-Count"] = total.ToString();
            Response.Headers["X-Page-Number"] = pageNumber.ToString();
            Response.Headers["X-Page-Size"] = pageSize.ToString();

            return Ok(facturas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener facturas");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Factura>> GetFactura(int id, CancellationToken ct = default)
    {
        try
        {
            // ✅ EQUIVALENTE A: GetByIdAsync(1, "Cliente", "OrdenServicio", "TipoPago")
            var factura = await _unitOfWork.Facturas.GetByIdAsync(id, ct, "Cliente,OrdenServicio,TipoPago");
            if (factura == null)
                return NotFound($"Factura con ID {id} no encontrada");

            return Ok(factura);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener factura {FacturaId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpGet("cliente/{clienteId}")]
    public async Task<ActionResult<IEnumerable<Factura>>> GetFacturasByCliente(int clienteId, CancellationToken ct = default)
    {
        try
        {
            // ✅ EQUIVALENTE A: GetFacturasByClienteAsync(clienteId: 5)
            var facturas = await _unitOfWork.Facturas.GetFacturasByClienteAsync(clienteId, ct);
            return Ok(facturas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener facturas del cliente {ClienteId}", clienteId);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpGet("ingresos")]
    public async Task<ActionResult<decimal>> GetIngresos([FromQuery] DateTime fechaDesde, [FromQuery] DateTime fechaHasta, CancellationToken ct = default)
    {
        try
        {
            // ✅ EQUIVALENTE A: GetTotalIngresosAsync(fechaDesde, fechaHasta)
            var total = await _unitOfWork.Facturas.GetTotalIngresosAsync(fechaDesde, fechaHasta, ct);
            return Ok(total);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al calcular ingresos entre {Desde} y {Hasta}", fechaDesde, fechaHasta);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<ActionResult<Factura>> CreateFactura(Factura factura, CancellationToken ct = default)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // ✅ VALIDACIONES DE NEGOCIO (mejor que IFacturaService)
            var orden = await _unitOfWork.OrdenesServicio.GetByIdAsync(factura.OrdenServicioId, ct);
            if (orden == null)
                return BadRequest("La orden de servicio especificada no existe");

            var cliente = await _unitOfWork.Clientes.GetByIdAsync(factura.ClienteId, ct);
            if (cliente == null)
                return BadRequest("El cliente especificado no existe");

            await _unitOfWork.Facturas.AddAsync(factura, ct);
            await _unitOfWork.SaveChangesAsync(ct); // ✅ TRANSACCIÓN

            _logger.LogInformation("Factura creada: {FacturaId}", factura.Id);
            return CreatedAtAction(nameof(GetFactura), new { id = factura.Id }, factura);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear factura");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<ActionResult<Factura>> UpdateFactura(int id, Factura factura, CancellationToken ct = default)
    {
        try
        {
            if (id != factura.Id)
                return BadRequest("El ID de la factura no coincide");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _unitOfWork.Facturas.GetByIdAsync(id, ct);
            if (existing == null)
                return NotFound($"Factura con ID {id} no encontrada");

            // Validar relaciones actualizadas
            var orden = await _unitOfWork.OrdenesServicio.GetByIdAsync(factura.OrdenServicioId, ct);
            if (orden == null)
                return BadRequest("La orden de servicio especificada no existe");

            var cliente = await _unitOfWork.Clientes.GetByIdAsync(factura.ClienteId, ct);
            if (cliente == null)
                return BadRequest("El cliente especificado no existe");

            existing.Fecha = factura.Fecha;
            existing.Total = factura.Total;
            existing.OrdenServicioId = factura.OrdenServicioId;
            existing.ClienteId = factura.ClienteId;
            existing.TipoPagoId = factura.TipoPagoId;

            await _unitOfWork.Facturas.UpdateAsync(existing, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Factura actualizada: {FacturaId}", id);
            return Ok(existing);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar factura {FacturaId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteFactura(int id, CancellationToken ct = default)
    {
        try
        {
            var deleted = await _unitOfWork.Facturas.DeleteAsync(id, ct);
            if (!deleted)
                return NotFound($"Factura con ID {id} no encontrada");

            await _unitOfWork.SaveChangesAsync(ct);
            _logger.LogInformation("Factura eliminada: {FacturaId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar factura {FacturaId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}