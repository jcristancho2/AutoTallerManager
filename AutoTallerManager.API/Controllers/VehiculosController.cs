using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Domain.Entities;

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

    /// <summary>
    /// Obtiene todos los vehículos con paginación y filtros
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Vehiculo>>> GetVehiculos(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? marca = null,
        [FromQuery] string? modelo = null,
        [FromQuery] int? clienteId = null,
        CancellationToken ct = default)
    {
        try
        {
            var vehiculos = await _unitOfWork.Vehiculos.GetAllAsync(
                filter: v => (string.IsNullOrEmpty(marca) || v.Marca.Contains(marca)) &&
                            (string.IsNullOrEmpty(modelo) || v.Modelo.Contains(modelo)) &&
                            (!clienteId.HasValue || v.ClienteId == clienteId),
                orderBy: q => q.OrderBy(v => v.Marca).ThenBy(v => v.Modelo),
                includeProperties: "Cliente",
                skip: (pageNumber - 1) * pageSize,
                take: pageSize,
                ct: ct);

            var totalCount = await _unitOfWork.Vehiculos.CountAsync(
                filter: v => (string.IsNullOrEmpty(marca) || v.Marca.Contains(marca)) &&
                            (string.IsNullOrEmpty(modelo) || v.Modelo.Contains(modelo)) &&
                            (!clienteId.HasValue || v.ClienteId == clienteId),
                ct: ct);

            Response.Headers.Add("X-Total-Count", totalCount.ToString());
            Response.Headers.Add("X-Page-Number", pageNumber.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());

            return Ok(vehiculos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener vehículos");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene un vehículo por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Vehiculo>> GetVehiculo(int id, CancellationToken ct = default)
    {
        try
        {
            var vehiculo = await _unitOfWork.Vehiculos.GetByIdAsync(id, ct, "Cliente", "OrdenesServicio");
            
            if (vehiculo == null)
            {
                return NotFound($"Vehículo con ID {id} no encontrado");
            }

            return Ok(vehiculo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener vehículo {VehiculoId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene vehículos por cliente
    /// </summary>
    [HttpGet("cliente/{clienteId}")]
    public async Task<ActionResult<IEnumerable<Vehiculo>>> GetVehiculosByCliente(
        int clienteId, CancellationToken ct = default)
    {
        try
        {
            var vehiculos = await _unitOfWork.Vehiculos.GetAllAsync(
                filter: v => v.ClienteId == clienteId,
                orderBy: q => q.OrderBy(v => v.Marca).ThenBy(v => v.Modelo),
                ct: ct);

            return Ok(vehiculos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener vehículos del cliente {ClienteId}", clienteId);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Busca vehículo por VIN
    /// </summary>
    [HttpGet("vin/{vin}")]
    public async Task<ActionResult<Vehiculo>> GetVehiculoByVin(string vin, CancellationToken ct = default)
    {
        try
        {
            var vehiculo = await _unitOfWork.Vehiculos.GetByVinAsync(vin, ct);
            
            if (vehiculo == null)
            {
                return NotFound($"Vehículo con VIN {vin} no encontrado");
            }

            return Ok(vehiculo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar vehículo por VIN {Vin}", vin);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Crea un nuevo vehículo
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<ActionResult<Vehiculo>> CreateVehiculo(Vehiculo vehiculo, CancellationToken ct = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validar que el cliente existe
            var clienteExists = await _unitOfWork.Clientes.ExistsAsync(c => c.Id == vehiculo.ClienteId, ct);
            if (!clienteExists)
            {
                return BadRequest("El cliente especificado no existe");
            }

            // Validar VIN único
            var vinExists = await _unitOfWork.Vehiculos.ExistsAsync(v => v.VIN == vehiculo.VIN, ct);
            if (vinExists)
            {
                return BadRequest("Ya existe un vehículo con este VIN");
            }

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

    /// <summary>
    /// Actualiza un vehículo existente
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<ActionResult<Vehiculo>> UpdateVehiculo(int id, Vehiculo vehiculo, CancellationToken ct = default)
    {
        try
        {
            if (id != vehiculo.Id)
            {
                return BadRequest("El ID del vehículo no coincide");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingVehiculo = await _unitOfWork.Vehiculos.GetByIdAsync(id, ct);
            if (existingVehiculo == null)
            {
                return NotFound($"Vehículo con ID {id} no encontrado");
            }

            // Validar VIN único (excluyendo el vehículo actual)
            var vinExists = await _unitOfWork.Vehiculos.ExistsAsync(
                v => v.VIN == vehiculo.VIN && v.Id != id, ct);
            if (vinExists)
            {
                return BadRequest("Ya existe otro vehículo con este VIN");
            }

            // Actualizar campos
            existingVehiculo.Marca = vehiculo.Marca;
            existingVehiculo.Modelo = vehiculo.Modelo;
            existingVehiculo.Año = vehiculo.Año;
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

    /// <summary>
    /// Elimina un vehículo
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteVehiculo(int id, CancellationToken ct = default)
    {
        try
        {
            var vehiculo = await _unitOfWork.Vehiculos.GetByIdAsync(id, ct, "OrdenesServicio");
            if (vehiculo == null)
            {
                return NotFound($"Vehículo con ID {id} no encontrado");
            }

            // Verificar si tiene órdenes de servicio activas
            var hasActiveOrders = vehiculo.OrdenesServicio.Any(o => 
                o.Estado != "Completada" && o.Estado != "Cancelada");

            if (hasActiveOrders)
            {
                return BadRequest("No se puede eliminar el vehículo porque tiene órdenes de servicio activas");
            }

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