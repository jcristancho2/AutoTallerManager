using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Authorization;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Domain.Enum;
using AutoTallerManager.API.DTOs.Request;
// using AutoTallerManager.API.Validators;

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
        [FromQuery] string? vehicleBrand = null,
        [FromQuery] string? vehicleModel = null,
        [FromQuery] int? customerId = null,
        CancellationToken ct = default)
    {
        try
        {
            var vehicles = await _unitOfWork.Vehicles.GetAllAsync(
                filter: v => (string.IsNullOrEmpty(vehicleBrand) || (v.VehicleBrand != null && v.VehicleBrand.Name!.Contains(vehicleBrand))) &&
                            (string.IsNullOrEmpty(vehicleModel) || (v.VehicleModel != null && v.VehicleModel.Name!.Contains(vehicleModel))) &&
                            (!customerId.HasValue || v.CustomerId == customerId.Value),
                orderBy: q => q.OrderBy(v => v.VehicleBrand!.Name)
                               .ThenBy(v => v.VehicleModel!.Name),
                includeProperties: "Customer,VehicleBrand,VehicleModel,VehicleType",
                skip: (pageNumber - 1) * pageSize,
                take: pageSize,
                ct: ct);

            var totalCount = await _unitOfWork.Vehicles.CountAsync(
                filter: v => (string.IsNullOrEmpty(vehicleBrand) || (v.VehicleBrand != null && v.VehicleBrand.Name!.Contains(vehicleBrand))) &&
                            (string.IsNullOrEmpty(vehicleModel) || (v.VehicleModel != null && v.VehicleModel.Name!.Contains(vehicleModel))) &&
                            (!customerId.HasValue || v.CustomerId == customerId.Value),
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
                id, ct, "Customer,ServiceOrders,VehicleBrand,VehicleModel,VehicleType");

            if (vehicle == null)
                return NotFound($"Vehículo con ID {id} no encontrado");

            return Ok(vehicle);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener vehículo {VehicleId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpGet("cliente/{CustomerId}")]
    public async Task<ActionResult<IEnumerable<Vehicle>>> GetVehiculosByCliente(
        int customerId, CancellationToken ct = default)
    {
        try
        {
            var vehicles = await _unitOfWork.Vehicles.GetAllAsync(
                filter: v => v.CustomerId == customerId,
                orderBy: q => q.OrderBy(v => v.VehicleBrand!.Name)
                               .ThenBy(v => v.VehicleModel!.Name),
                includeProperties: "VehicleBrand,VehicleModel",
                ct: ct);

            return Ok(vehicles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener vehículos del cliente {CustomerId}", customerId);
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
            // Validación básica con DataAnnotations ya aplicada en VehicleRequest

            // Validaciones de negocio
            var customerExists = await _unitOfWork.Customer.ExistsAsync(c => c.Id == request.CustomerId, ct);
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
                VehicleBrandId = request.VehicleBrandId,
                VehicleModelId = request.VehicleModelId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Vehicles.AddAsync(vehicle, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Vehículo creado: {VehiculoId} - {VIN}", vehicle.Id, vehicle.VIN);

            // Obtener el vehículo creado con relaciones para la respuesta
            var vehiculoCreado = await _unitOfWork.Vehicles.GetByIdAsync(vehicle.Id, ct, "Customer,VehicleBrand,VehicleModel,VehicleType");

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
                v => v.VIN == vehicle.VIN && v.Id != id, ct);
            if (vinExists)
                return BadRequest("Ya existe otro vehículo con este VIN");

            existingVehicle.VehicleBrandId = vehicle.VehicleBrandId;
            existingVehicle.VehicleModelId = vehicle.VehicleModelId;
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
            _logger.LogError(ex, "Error al actualizar vehículo {VehiculoId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteVehicle(int id, CancellationToken ct = default)
    {
        try
        {
            var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(id, ct, "ServiceOrders.ServiceStatus");
            if (vehicle == null)
                return NotFound($"Vehículo con ID {id} no encontrado");

            var hasActiveOrders = vehicle.ServiceOrders != null &&
                vehicle.ServiceOrders.Any(o =>
                    o.ServiceStatus != null &&
                    o.ServiceStatus.ServiceStatusName != "Completada" &&
                    o.ServiceStatus.ServiceStatusName != "Cancelada");

            if (hasActiveOrders)
                return BadRequest("No se puede eliminar el vehículo porque tiene órdenes de servicio activas");

            _unitOfWork.Vehicles.Delete(vehicle);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Vehículo eliminado: {VehiculoId} - {VIN}", id, vehicle.VIN);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar vehículo {VehiculoId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}