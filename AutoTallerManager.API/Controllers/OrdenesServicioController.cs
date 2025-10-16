using System;
using Microsoft.AspNetCore.RateLimiting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Domain.Entities;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using AutoTallerManager.Application.Features.OrdenesServicio.Commands;
using AutoTallerManager.Application.Features.Facturas.Commands;
using AutoTallerManager.Application.Dtos;
using AutoTallerManager.Infrastructure.Data;

namespace AutoTallerManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[EnableRateLimiting("OrdenesServicio")]
public class OrdenesServicioController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OrdenesServicioController> _logger;
    private readonly IMediator _mediator;
    private readonly AutoTallerDbContext _context;

    public OrdenesServicioController(IUnitOfWork unitOfWork, ILogger<OrdenesServicioController> logger, IMediator mediator, AutoTallerDbContext context)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mediator = mediator;
        _context = context;
    }

    public class UpdateEstadoOrdenRequest
    {
        public int EstadoId { get; set; }
    }

    public class CerrarOrdenRequest
    {
        public int TipoPagoId { get; set; }
    }

    public record CerrarOrdenResponse(int FacturaId, decimal Total);

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
    public async Task<ActionResult<OrdenServicioResponseDto>> GetOrdenServicio(int id)
    {
        var orden = await _context.OrdenesServicio.FindAsync(id);
        if (orden == null)
            return NotFound();

        return Ok(MapToDto(orden));
    }

    [HttpPost("{id}/detalles")]
    [Authorize(Roles = "Admin,Mecanico")]
    public async Task<ActionResult<DetalleOrden>> AddDetalle(int id, [FromBody] DetalleOrden detalle, CancellationToken ct = default)
    {
        try
        {
            var orden = await _unitOfWork.OrdenesServicio.GetByIdAsync(id, ct, "Vehiculo");
            if (orden == null)
                return NotFound($"Orden de servicio con ID {id} no encontrada");

            detalle.OrdenServicioId = id;

            if (detalle.RepuestoId.HasValue && detalle.RepuestoId.Value > 0)
            {
                var repuesto = await _unitOfWork.Repuestos.GetByIdAsync(detalle.RepuestoId.Value, ct);
                if (repuesto == null) return BadRequest("Repuesto no existe");
                if (detalle.Cantidad <= 0) return BadRequest("Cantidad debe ser mayor a 0");
                if (repuesto.Stock < detalle.Cantidad) return BadRequest("Stock insuficiente del repuesto");

                repuesto.Stock = repuesto.Stock - detalle.Cantidad;
                await _unitOfWork.Repuestos.UpdateAsync(repuesto, ct);
            }

            await _unitOfWork.DetallesOrden.AddAsync(detalle, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Detalle agregado a orden {OrdenId}: DetalleId {DetalleOrdenId}", id, detalle.DetalleOrdenId);
            return CreatedAtAction(nameof(GetOrdenServicio), new { id = id }, detalle);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al agregar detalle a orden {OrdenId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPut("{id}/detalles/{detalleId}")]
    [Authorize(Roles = "Admin,Mecanico")]
    public async Task<ActionResult<DetalleOrden>> UpdateDetalle(int id, int detalleId, [FromBody] DetalleOrden detalle, CancellationToken ct = default)
    {
        try
        {
            var existente = await _unitOfWork.DetallesOrden.GetByIdAsync(detalleId, id, ct);
            if (existente == null) return NotFound("Detalle no encontrado");

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
            _logger.LogError(ex, "Error al actualizar detalle {DetalleId} de orden {OrdenId}", detalleId, id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpDelete("{id}/detalles/{detalleId}")]
    [Authorize(Roles = "Admin,Mecanico")]
    public async Task<ActionResult> DeleteDetalle(int id, int detalleId, CancellationToken ct = default)
    {
        try
        {
            var existente = await _unitOfWork.DetallesOrden.GetByIdAsync(detalleId, id, ct);
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

            var deleted = await _unitOfWork.DetallesOrden.DeleteAsync(detalleId, id, ct);
            if (!deleted) return NotFound();

            await _unitOfWork.SaveChangesAsync(ct);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar detalle {DetalleId} de orden {OrdenId}", detalleId, id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPut("{id}/estado")]
    [Authorize(Roles = "Admin,Mecanico,Recepcionista")]
    public async Task<ActionResult> UpdateEstado(int id, [FromBody] UpdateEstadoOrdenRequest request, CancellationToken ct = default)
    {
        try
        {
            var orden = await _unitOfWork.OrdenesServicio.GetByIdAsync(id, ct);
            if (orden == null) return NotFound("Orden de servicio no encontrada");

            orden.EstadoId = request.EstadoId;
            await _unitOfWork.OrdenesServicio.UpdateAsync(orden, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar estado de orden {OrdenId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPost("{id}/cerrar")]
    [Authorize(Roles = "Admin,Mecanico")]
    public async Task<ActionResult> CerrarOrden(int id, [FromBody] CerrarOrdenRequest request, CancellationToken ct = default)
    {
        try
        {
            var orden = await _unitOfWork.OrdenesServicio.GetByIdAsync(id, ct, "Vehiculo", "DetallesOrden", "Vehiculo.Cliente");
            if (orden == null) return NotFound("Orden de servicio no encontrada");

            var detalles = await _unitOfWork.DetallesOrden.GetDetallesByOrdenAsync(id, ct);
            var total = detalles.Sum(d => (d.PrecioUnitario * d.Cantidad) + d.PrecioManoDeObra);

            int clienteId = 0;
            if (orden.Vehiculo != null)
            {
                clienteId = orden.Vehiculo.ClienteId;
                if (clienteId == 0 && orden.Vehiculo.Cliente != null)
                {
                    clienteId = orden.Vehiculo.Cliente.Id;
                }
            }

            if (clienteId == 0 && orden.VehiculoId > 0)
            {
                var veh = await _unitOfWork.Vehiculos.GetByIdAsync(orden.VehiculoId, ct, "Cliente");
                clienteId = veh?.ClienteId ?? veh?.Cliente?.Id ?? 0;
            }

            if (clienteId == 0)
            {
                if (orden.Vehiculo?.Cliente != null)
                {
                    var cnav = orden.Vehiculo.Cliente;
                    if (!string.IsNullOrWhiteSpace(cnav.NombreCompleto))
                    {
                        var matches = await _unitOfWork.Clientes.GetAllAsync(filter: c => c.NombreCompleto == cnav.NombreCompleto, ct: ct);
                        var m = matches.FirstOrDefault();
                        if (m != null) clienteId = m.Id;
                    }

                    if (clienteId == 0 && !string.IsNullOrWhiteSpace(cnav.Email))
                    {
                        var matches = await _unitOfWork.Clientes.GetAllAsync(filter: c => c.Email == cnav.Email, ct: ct);
                        var m = matches.FirstOrDefault();
                        if (m != null) clienteId = m.Id;
                    }
                }

                if (clienteId == 0)
                {
                    var clientes = await _unitOfWork.Clientes.GetAllAsync(ct: ct);
                    var anyValid = clientes.FirstOrDefault(c => c.Id > 0);
                    if (anyValid != null)
                        clienteId = anyValid.Id;
                    else
                    {
                        var anyClient = clientes.FirstOrDefault();
                        clienteId = anyClient?.Id ?? 0;
                    }
                }
            }

            if (clienteId == 0)
            {
                var clientes = await _unitOfWork.Clientes.GetAllAsync(ct: ct);
                if (clientes != null && clientes.Any())
                {
                    clienteId = clientes.First().Id;
                }
                else
                {
                    var diag = new
                    {
                        Message = "No se puede determinar el cliente de la orden",
                        OrdenId = id,
                        OrdenVehiculoId = orden.VehiculoId,
                        OrdenHasVehiculo = orden.Vehiculo != null,
                        OrdenVehiculo_ClienteId = orden.Vehiculo?.ClienteId,
                        OrdenVehiculo_ClientePresent = orden.Vehiculo?.Cliente != null,
                        ClientesCount = 0,
                        FirstClienteId = 0
                    };

                    return BadRequest(diag);
                }
            }

            var factura = new Factura
            {
                Fecha = DateTime.UtcNow,
                Total = total,
                OrdenServicioId = id,
                ClienteId = clienteId,
                TipoPagoId = request.TipoPagoId
            };

            await _unitOfWork.Facturas.AddAsync(factura, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return Ok(new CerrarOrdenResponse(factura.Id, factura.Total));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cerrar orden {OrdenId}", id);
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

            var vehiculoExists = await _unitOfWork.Vehiculos.ExistsAsync(v => v.Id == ordenServicio.VehiculoId, ct);
            if (!vehiculoExists)
                return BadRequest("El vehículo especificado no existe");

            var mecanicoExists = await _unitOfWork.Usuarios.GetByIdAsync(ordenServicio.MecanicoId, ct);
            if (mecanicoExists == null)
                return BadRequest("El mecánico especificado no existe");

            var tipoExists = await _unitOfWork.OrdenesServicio.CountAsync(o => o.TipoServId == ordenServicio.TipoServId, ct);

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
    public async Task<ActionResult<OrdenServicioResponseDto>> UpdateOrdenServicio(int id, OrdenServicioUpdateDto updateDto)
    {
        var orden = await _context.OrdenesServicio.FindAsync(id);
        if (orden == null)
            return NotFound();

        var providedVersion = string.IsNullOrEmpty(updateDto.RowVersion) 
            ? null 
            : Convert.FromBase64String(updateDto.RowVersion);
        var expectedVersion = orden.RowVersion;

        if (!ByteArraysEqual(providedVersion, expectedVersion))
        {
            var conflictResponse = new
            {
                message = "Concurrency conflict",
                entity = "OrdenServicio",
                id = id,
                expectedVersion = Convert.ToBase64String(expectedVersion),
                providedVersion = updateDto.RowVersion
            };
            return Conflict(conflictResponse);
        }

        orden.NumeroOrden = updateDto.NumeroOrden;
        orden.Estado = updateDto.Estado;
        orden.Total = updateDto.Total;
        orden.FechaModificacion = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
        {
            var conflictResponse = new
            {
                message = "Concurrency conflict",
                entity = "OrdenServicio",
                id = id,
                expectedVersion = Convert.ToBase64String(expectedVersion),
                providedVersion = updateDto.RowVersion
            };
            return Conflict(conflictResponse);
        }

        return Ok(MapToDto(orden));
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

    [HttpPost("crear-orden")]
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<ActionResult<int>> CrearOrdenServicio(
        [FromBody] CrearOrdenServicioCommand command,
        CancellationToken ct = default)
    {
        try
        {
            var ordenId = await _mediator.Send(command, ct);
            
            _logger.LogInformation("Orden de servicio creada: {OrdenId}", ordenId);
            
            return CreatedAtAction(nameof(GetOrdenServicio), new { id = ordenId }, new { ordenId });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error de validación al crear orden de servicio");
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Recurso no encontrado al crear orden de servicio");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear orden de servicio");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPut("{id}/actualizar-trabajo")]
    [Authorize(Roles = "Admin,Mecanico")]
    public async Task<ActionResult> ActualizarOrdenConTrabajoRealizado(
        int id,
        [FromBody] ActualizarOrdenConTrabajoRealizadoCommand command,
        CancellationToken ct = default)
    {
        try
        {
            command = command with { OrdenId = id };
            var resultado = await _mediator.Send(command, ct);
            
            _logger.LogInformation("Trabajo actualizado en orden: {OrdenId}", id);
            
            return Ok(new { success = resultado, message = "Trabajo actualizado correctamente" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error de validación al actualizar trabajo en orden {OrdenId}", id);
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Recurso no encontrado al actualizar trabajo en orden {OrdenId}", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar trabajo en orden {OrdenId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPost("{id}/cerrar-orden")]
    [Authorize(Roles = "Admin,Mecanico")]
    public async Task<ActionResult<CerrarOrdenServicioResponse>> CerrarOrdenServicio(
        int id,
        [FromBody] CerrarOrdenServicioCommand command,
        CancellationToken ct = default)
    {
        try
        {
            command = command with { OrdenId = id };
            var resultado = await _mediator.Send(command, ct);
            
            _logger.LogInformation("Orden cerrada: {OrdenId}, Factura: {FacturaId}", id, resultado.FacturaId);
            
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error de validación al cerrar orden {OrdenId}", id);
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Recurso no encontrado al cerrar orden {OrdenId}", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cerrar orden {OrdenId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPost("generar-factura")]
    [Authorize(Roles = "Admin,Mecanico")]
    public async Task<ActionResult<GenerarFacturaResponse>> GenerarFactura(
        [FromBody] GenerarFacturaCommand command,
        CancellationToken ct = default)
    {
        try
        {
            var resultado = await _mediator.Send(command, ct);
            
            _logger.LogInformation("Factura generada: {FacturaId} para orden {OrdenId}", resultado.FacturaId, command.OrdenServicioId);
            
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error de validación al generar factura para orden {OrdenId}", command.OrdenServicioId);
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Recurso no encontrado al generar factura para orden {OrdenId}", command.OrdenServicioId);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar factura para orden {OrdenId}", command.OrdenServicioId);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    private OrdenServicioResponseDto MapToDto(OrdenServicio orden)
    {
        return new OrdenServicioResponseDto
        {
            Id = orden.Id,
            NumeroOrden = orden.NumeroOrden,
            Estado = orden.Estado,
            Total = orden.Total,
            RowVersion = Convert.ToBase64String(orden.RowVersion)
        };
    }

    private bool ByteArraysEqual(byte[] a, byte[] b)
    {
        if (a == null && b == null) return true;
        if (a == null || b == null) return false;
        return System.Linq.Enumerable.SequenceEqual(a, b);
    }
}