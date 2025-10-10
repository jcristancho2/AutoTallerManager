/*
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Domain.Enum;

namespace AutoTallerManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VehiculosController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<VehiculosController> _logger;

    public VehiculosController(IUnitOfWork unitOfWork, ILogger<VehiculosController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Vehiculo>>> GetVehiculos(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? MarcaVehiculo = null,
        [FromQuery] string? ModeloVehiculo = null,
        [FromQuery] int? clienteId = null,
        CancellationToken ct = default)
    {
        try
        {
            var vehiculos = await _unitOfWork.Vehiculos.GetAllAsync(
                filter: v => (string.IsNullOrEmpty(MarcaVehiculo) || (v.MarcaVehiculo != null && v.MarcaVehiculo.Nombre.Contains(MarcaVehiculo))) &&
                            (string.IsNullOrEmpty(ModeloVehiculo) || (v.ModeloVehiculo != null && v.ModeloVehiculo.Nombre.Contains(ModeloVehiculo))) &&
                            (!clienteId.HasValue || v.ClienteId == clienteId),
                orderBy: q => q.OrderBy(v => v.MarcaVehiculo!.Nombre)
                               .ThenBy(v => v.ModeloVehiculo!.Nombre),
                includeProperties: "Cliente,MarcaVehiculo,ModeloVehiculo",
                skip: (pageNumber - 1) * pageSize,
                take: pageSize,
                ct: ct);

            var totalCount = await _unitOfWork.Vehiculos.CountAsync(
                filter: v => (string.IsNullOrEmpty(MarcaVehiculo) || (v.MarcaVehiculo != null && v.MarcaVehiculo.Nombre.Contains(MarcaVehiculo))) &&
                            (string.IsNullOrEmpty(ModeloVehiculo) || (v.ModeloVehiculo != null && v.ModeloVehiculo.Nombre.Contains(ModeloVehiculo))) &&
                            (!clienteId.HasValue || v.ClienteId == clienteId),
                ct: ct);

            Response.Headers["X-Total-Count"] = totalCount.ToString();
            Response.Headers["X-Page-Number"] = pageNumber.ToString();
            Response.Headers["X-Page-Size"] = pageSize.ToString();

            return Ok(vehiculos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener vehículos");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Vehiculo>> GetVehiculo(int id, CancellationToken ct = default)
    {
        try
        {
            var vehiculo = await _unitOfWork.Vehiculos.GetByIdAsync(
                id, ct, "Cliente,OrdenesServicio,MarcaVehiculo,ModeloVehiculo");

            if (vehiculo == null)
                return NotFound($"Vehículo con ID {id} no encontrado");

            return Ok(vehiculo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener vehículo {VehiculoId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpGet("cliente/{clienteId}")]
    public async Task<ActionResult<IEnumerable<Vehiculo>>> GetVehiculosByCliente(
        int clienteId, CancellationToken ct = default)
    {
        try
        {
            var vehiculos = await _unitOfWork.Vehiculos.GetAllAsync(
                filter: v => v.ClienteId == clienteId,
                orderBy: q => q.OrderBy(v => v.MarcaVehiculo!.Nombre)
                               .ThenBy(v => v.ModeloVehiculo!.Nombre),
                includeProperties: "MarcaVehiculo,ModeloVehiculo",
                ct: ct);

            return Ok(vehiculos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener vehículos del cliente {ClienteId}", clienteId);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpGet("vin/{vin}")]
    public async Task<ActionResult<Vehiculo>> GetVehiculoByVin(string vin, CancellationToken ct = default)
    {
        try
        {
            var vehiculo = await _unitOfWork.Vehiculos.GetByVinAsync(vin, ct);

            if (vehiculo == null)
                return NotFound($"Vehículo con VIN {vin} no encontrado");

            return Ok(vehiculo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar vehículo por VIN {Vin}", vin);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<ActionResult<Vehiculo>> CreateVehiculo(Vehiculo vehiculo, CancellationToken ct = default)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var clienteExists = await _unitOfWork.Clientes.ExistsAsync(c => c.Id == vehiculo.ClienteId, ct);
            if (!clienteExists)
                return BadRequest("El cliente especificado no existe");

            var vinExists = await _unitOfWork.Vehiculos.ExistsAsync(v => v.VIN == vehiculo.VIN, ct);
            if (vinExists)
                return BadRequest("Ya existe un vehículo con este VIN");

            await _unitOfWork.Vehiculos.AddAsync(vehiculo, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Vehículo creado: {VehiculoId} - {VIN}", vehiculo.Id, vehiculo.VIN);
            return CreatedAtAction(nameof(GetVehiculo), new { id = vehiculo.Id }, vehiculo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear vehículo");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<ActionResult<Vehiculo>> UpdateVehiculo(int id, Vehiculo vehiculo, CancellationToken ct = default)
    {
        try
        {
            if (id != vehiculo.Id)
                return BadRequest("El ID del vehículo no coincide");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingVehiculo = await _unitOfWork.Vehiculos.GetByIdAsync(id, ct);
            if (existingVehiculo == null)
                return NotFound($"Vehículo con ID {id} no encontrado");

            var vinExists = await _unitOfWork.Vehiculos.ExistsAsync(
                v => v.VIN == vehiculo.VIN && v.Id != id, ct);
            if (vinExists)
                return BadRequest("Ya existe otro vehículo con este VIN");

            existingVehiculo.MarcaVehiculoId = vehiculo.MarcaVehiculoId;
            existingVehiculo.ModeloVehiculoId = vehiculo.ModeloVehiculoId;
            existingVehiculo.Anio = vehiculo.Anio;
            existingVehiculo.VIN = vehiculo.VIN;
            existingVehiculo.Kilometraje = vehiculo.Kilometraje;
            existingVehiculo.ClienteId = vehiculo.ClienteId;

            _unitOfWork.Vehiculos.Update(existingVehiculo);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Vehículo actualizado: {VehiculoId} - {VIN}", id, vehiculo.VIN);
            return Ok(existingVehiculo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar vehículo {VehiculoId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteVehiculo(int id, CancellationToken ct = default)
    {
        try
        {
            var vehiculo = await _unitOfWork.Vehiculos.GetByIdAsync(id, ct, "OrdenesServicio");
            if (vehiculo == null)
                return NotFound($"Vehículo con ID {id} no encontrado");

            var hasActiveOrders = vehiculo.OrdenesServicio != null &&
                vehiculo.OrdenesServicio.Any(o =>
                    o.Estado != null &&
                    o.Estado.NombreEstServ != EstadoOrden.Completada.ToString() &&
                    o.Estado.NombreEstServ != EstadoOrden.Cancelada.ToString());

            if (hasActiveOrders)
                return BadRequest("No se puede eliminar el vehículo porque tiene órdenes de servicio activas");

            _unitOfWork.Vehiculos.Delete(vehiculo);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Vehículo eliminado: {VehiculoId} - {VIN}", id, vehiculo.VIN);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar vehículo {VehiculoId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}
<<<<<<< HEAD
*/
=======
>>>>>>> develop
