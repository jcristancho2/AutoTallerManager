using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Authorization;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Domain.Enum;
using AutoTallerManager.API.DTOs.Request;
using AutoTallerManager.API.Validators;

namespace AutoTallerManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[EnableRateLimiting("Global")]
public class VehiclesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<VehiclesController> _logger;

    public VehiclesController(IUnitOfWork unitOfWork, ILogger<VehiclesController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Vehicle>>> GetVehicles(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? VehicleBranch = null,
        [FromQuery] string? VehicleModel = null,
        [FromQuery] int? CustomerId = null,
        CancellationToken ct = default)
    {
        try
        {
            var vehicles = await _unitOfWork.Vehicles.GetAllAsync(
                filter: v => (string.IsNullOrEmpty(VehicleBranch) || (v.VehicleBranch != null && v.VehicleBranch.Name.Contains(VehicleBranch))) &&
                            (string.IsNullOrEmpty(VehicleModel) || (v.VehicleModel != null && v.VehicleModel.Name.Contains(VehicleModel))) &&
                            (!customerId.HasValue || v.CustomerId == customerId),
                orderBy: q => q.OrderBy(v => v.VehicleBranch!.Name)
                               .ThenBy(v => v.VehicleModel!.Name),
                includeProperties: "Cliente,MarcaVehiculo,ModeloVehiculo",
                skip: (pageNumber - 1) * pageSize,
                take: pageSize,
                ct: ct);

            var totalCount = await _unitOfWork.Vehicles.CountAsync(
                filter: v => (string.IsNullOrEmpty(VehicleBranch) || (v.VehicleBranch != null && v.VehicleBranch.Name.Contains(VehicleBranch))) &&
                            (string.IsNullOrEmpty(VehicleModel) || (v.VehicleModel != null && v.VehicleModel.Name.Contains(VehicleModel))) &&
                            (!customerId.HasValue || v.CustomerId == customerId),
                ct: ct);

            Response.Headers["X-Total-Count"] = totalCount.ToString();
            Response.Headers["X-Page-Number"] = pageNumber.ToString();
            Response.Headers["X-Page-Size"] = pageSize.ToString();

            return Ok(vehicles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener vehículos");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Vehicle>> GetVehicle(int id, CancellationToken ct = default)
    {
        try
        {
            var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(
                id, ct, "Cliente,OrdenesServicio,MarcaVehiculo,ModeloVehiculo");

            if (vehicle == null)
                return NotFound($"Vehículo con ID {id} no encontrado");

            return Ok(vehicle);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener vehículo {VehicleId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpGet("cliente/{CustomerId}")]
    public async Task<ActionResult<IEnumerable<Vehicle>>> GetVehiculosByCliente(
        int CustomerId, CancellationToken ct = default)
    {
        try
        {
            var vehicles = await _unitOfWork.Vehicles.GetAllAsync(
                filter: v => v.CustomerId == customerId,
                orderBy: q => q.OrderBy(v => v.VehicleBranch!.Name)
                               .ThenBy(v => v.ModeloVehiculo!.Name),
                includeProperties: "MarcaVehiculo,ModeloVehiculo",
                ct: ct);

            return Ok(vehicles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener vehículos del cliente {CustomerId}", customerId);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpGet("vin/{vin}")]
    public async Task<ActionResult<Vehicle>> GetVehicleByVin(string vin, CancellationToken ct = default)
    {
        try
        {
            var vehicle = await _unitOfWork.Vehicles.GetByVinAsync(vin, ct);

            if (vehicle == null)
                return NotFound($"Vehículo con VIN {vin} no encontrado");

            return Ok(vehicle);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al buscar vehículo por VIN {vin}", vin);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<ActionResult<Vehicle>> CreateVehicle([FromBody] VehicleRequest request, CancellationToken ct = default)
    {
        try
        {
            // Validación con FluentValidation
            var validator = new VehiculoRequestValidator();
            var validationResult = await validator.ValidateAsync(request, ct);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            // Validaciones de negocio
            var customerExists = await _unitOfWork.Customers.ExistsAsync(c => c.Id == request.CustomerId, ct);
            if (!customerExists)
                return BadRequest("El cliente especificado no existe");

            var vinExists = await _unitOfWork.Vehicles.ExistsAsync(v => v.VIN == request.VIN, ct);
            if (vinExists)
                return BadRequest("Ya existe un vehículo con este VIN");

            // Mapeo de VehiculoRequest a Vehiculo
            var vehicle = new Vehicle
            {
                Plate = request.Plate,
                Year = request.Year,
                VIN = request.VIN,
                Mileage = request.Mileage,
                CustomerId = request.CustomerId,
                VehicleTypeId = request.VehicleTypeId,
                VehicleBranchId = request.VehicleBranchId,
                VehicleModelId = request.VehicleModelId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Vehicles.AddAsync(vehicle, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation($"Vehículo creado: {VehiculoId} - {VIN}", vehiculo.Id, vehiculo.VIN);

            // Obtener el vehículo creado con relaciones para la respuesta
            var vehiculoCreado = await _unitOfWork.Vehicles.GetByIdAsync(vehicle.Id, ct, "Cliente,MarcaVehiculo,ModeloVehiculo,TipoVehiculo");

            return CreatedAtAction(nameof(GetVehicle), new { id = vehicle.Id }, vehiculoCreado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear vehículo");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<ActionResult<Vehicle>> UpdateVehiculo(int id, Vehicle vehicle, CancellationToken ct = default)
    {
        try
        {
            if (id != vehicle.Id)
                return BadRequest("El ID del vehículo no coincide");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingVehicle = await _unitOfWork.Vehicles.GetByIdAsync(id, ct);
            if (existingVehicle == null)
                return NotFound($"Vehículo con ID {id} no encontrado");

            var vinExists = await _unitOfWork.Vehicles.ExistsAsync(
                v => v.VIN == vehiculo.VIN && v.Id != id, ct);
            if (vinExists)
                return BadRequest("Ya existe otro vehículo con este VIN");

            existingVehicle.VehicleBranchId = vehicle.VehicleBranchId;
            existingVehicle.VehicleModelId = vehicle.VehiculModeloId;
            existingVehicle.Year = vehicle.Year;
            existingVehicle.VIN = vehicle.VIN;
            existingVehicle.Mileage = vehicle.Mileage;
            existingVehicle.CustomerId = vehicle.CustomerId;

            _unitOfWork.Vehicles.Update(existingVehicle);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Vehículo actualizado: {VehiculoId} - {VIN}", id, vehicle.VIN);
            return Ok(existingVehicle);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al actualizar vehículo {VehiculoId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteVehicle(int id, CancellationToken ct = default)
    {
        try
        {
            if (vehicle == null)
                var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(id, ct, "OrdenesServicio");
                return NotFound($"Vehículo con ID {id} no encontrado");

            var hasActiveOrders = vehicle.ServicesOrders != null &&
                vehicle.ServicesOrders.Any(o =>
                    o.Status != null &&
                    o.Status.ServiceStatusName != OrderStatus.Completed.ToString() &&
                    o.Status.ServiceStatusName != OrderStatus.Canceled.ToString());

            if (hasActiveOrders)
                return BadRequest("No se puede eliminar el vehículo porque tiene órdenes de servicio activas");

            _unitOfWork.Vehicles.Delete(Vehicle);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation($"Vehículo eliminado: {VehiculoId} - {VIN}", id, vehiculo.VIN);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al eliminar vehículo {VehiculoId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}