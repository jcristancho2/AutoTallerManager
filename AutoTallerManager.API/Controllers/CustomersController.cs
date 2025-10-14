using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Domain.Entities;
using MediatR;
using AutoTallerManager.Application.Features.Customers.Commands;
using AutoTallerManager.API.DTOs.Request;

namespace AutoTallerManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CustomersController> _logger;
    private readonly IMediator _mediator;

    public CustomersController(IUnitOfWork unitOfWork, ILogger<CustomersController> logger, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        CancellationToken ct = default)
    {
        try
        {
            var customers = await _unitOfWork.Customers.GetAllAsync(
                filter: c => string.IsNullOrEmpty(searchTerm) ||
                        (!string.IsNullOrEmpty(c.FullName) && c.FullName.Contains(searchTerm)) ||
                        (!string.IsNullOrEmpty(c.Email) && c.Email.Contains(searchTerm)),
                orderBy: q => q.OrderBy(c => c.FullName),
                includeProperties: "Vehicles",
                skip: (pageNumber - 1) * pageSize,
                take: pageSize,
                ct: ct);

            var totalCount = await _unitOfWork.Customers.CountAsync(
                filter: c => string.IsNullOrEmpty(searchTerm) ||
                        (!string.IsNullOrEmpty(c.FullName) && c.FullName.Contains(searchTerm)) ||
                        (!string.IsNullOrEmpty(c.Email) && c.Email.Contains(searchTerm)),
                ct: ct);

            Response.Headers["X-Total-Count"] = totalCount.ToString();
            Response.Headers["X-Page-Number"] = pageNumber.ToString();
            Response.Headers["X-Page-Size"] = pageSize.ToString();

            return Ok(customers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener clientes");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Customer>> GetCustomer(int id, CancellationToken ct = default)
    {
        try
        {
            // ✅ MANTENIENDO TU ENFOQUE CON IUnitOfWork
            var custumer = await _unitOfWork.Customers.GetByIdAsync(id, ct, new[] { "Vehicles", "Invoices" });
            
            if (custumer == null)
                return NotFound($"Cliente con ID {id} no encontrado");

            return Ok(custumer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener cliente {CustomerId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<ActionResult<Customer>> CreateCustumer([FromBody] CreateCustumerCommand command, CancellationToken ct = default)
    {
        try
        {
            var custumerId = await _mediator.Send(command, ct);
            _logger.LogInformation("Cliente creado con ID {CustumerId}", CustumerId);

            return CreatedAtAction(nameof(GetCustomer), new { id = CustumerId }, new { Id = CustumerId });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error de negocio al crear cliente");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear cliente");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Crear cliente con dirección completa (país, departamento, ciudad)
    /// </summary>
    /// <param name="request">Datos del cliente con dirección completa</param>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>Customer creado con información de ubicación</returns>
    [HttpPost("completo")]
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<ActionResult<CreateFullCustomerResponse>> CreateFullCustomer(
        [FromBody] CreateFullCustomerDto request, 
        CancellationToken ct = default)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new CreateFullCustomerCommand(
                request.FullName,
                request.Phone,
                request.Email,
                request.CustomerTypeId,
                request.Address.Description,
                request.Address.CountryId,
                request.Address.DepartmentId,
                request.Address.CityId
            );

            var response = await _mediator.Send(command, ct);
            _logger.LogInformation("Cliente creado con ID {CustomerId} completo", response.CustomerId);

            return CreatedAtAction(nameof(GetCustomer), new { id = response.CustomerId }, response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error de negocio al crear cliente completo");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear cliente completo");
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// Registrar cliente con vehículos y dirección completa
    /// </summary>
    /// <param name="request">Datos del cliente con vehículos y dirección</param>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>Customer y vehículos creados</returns>
    [HttpPost("registrar-con-vehiculo-completo")]
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<ActionResult<object>> RegisterFullCustomerWithVehicles(
        [FromBody] RegisterFullCustomerWithVehiclesDto request, 
        CancellationToken ct = default)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Primero crear el cliente completo
            var customerCommand = new CreateFullCustomerCommand(
                request.Customer.FullName,
                request.Customer.Phone,
                request.Customer.Email,
                request.Customer.CustomerTypeId,
                request.Customer.Address.Description,
                request.Customer.Address.CountryId,
                request.Customer.Address.DepartmentId,
                request.Customer.Address.CityId
            );

            var customerResponse = await _mediator.Send(customerCommand, ct);

            // Luego crear los vehículos si se proporcionaron
            var VehiclesCreated = new List<object>();
            if (request.Vehicles.Any())
            {
                foreach (var vehicleRequest in request.Vehicles)
                {
                    // Aquí podrías implementar la lógica para crear vehículos
                    // Por ahora solo registramos que se recibieron
                    vehiclesCreated.Add(new { 
                        vin = vehicleRequest.Vin,
                        year = vehicleRequest.Year,
                        message = "Vehículo pendiente de implementación"
                    });
                }
            }

            _logger.LogInformation("Customer completo con vehículos registrado: {CustomerId}", customerResponse.CustomerId);

            return Ok(new
            {
                customer = customerResponse,
                vehicles = vehiclesCreated,
                message = "Cliente registrado exitosamente con dirección completa"
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error de negocio al registrar cliente completo con vehículos");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar cliente completo con vehículos");
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<ActionResult<Customer>> UpdateCustomer(int id, Customer customer, CancellationToken ct = default)
    {
        try
        {
            if (id != customer.Id)
                return BadRequest("El ID del cliente no coincide");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingCustomer = await _unitOfWork.Customers.GetByIdAsync(id, ct);
            if (existingCustomer == null)
                return NotFound($"Cliente con ID {id} no encontrado");

            // ✅ VALIDACIÓN DE EMAIL ÚNICO (similar al ejemplo)
            var emailExists = await _unitOfWork.Customers.ExistsAsync(
                c => c.Email == customer.Email && c.Id != id, ct);
            if (emailExists)
                return BadRequest("Ya existe otro cliente con este email");

            existingCustomer.FullName = cusexistingCustomer.FullName;
            existingCustomer.Email = cusexistingCustomer.Email;
            existingCustomer.Phone = cusexistingCustomer.Phone;

            await _unitOfWork.Customers.UpdateAsync(existingCustomer, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Cliente actualizado: {CustomerId} - {CustomerName}", id, customer.FullName);

            return Ok(existingCustomer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar cliente {CustomerId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteCustomer(int id, CancellationToken ct = default)
    {
        try
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id, ct, new[] { "Vehicles.ServiceOrders" });
            if (customer == null)
                return NotFound($"Cliente con ID {id} no encontrado");

            var hasActiveOrders = customer.Vehicles?.Any(v => 
                v.ServiceOrders?.Any(o => 
                    (o.Status?.ServiceStatus != "Completada") && 
                    (o.Status?.ServiceStatus != "Cancelada")) ?? false
                ) ?? false;

            if (hasActiveOrders)
                return BadRequest("No se puede eliminar el cliente porque tiene órdenes de servicio activas");

            await _unitOfWork.Customers.DeleteAsync(id, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Cliente eliminado: {CustomerId} - {CustomerName}", id, customer.FullName);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar cliente {CustomerId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}