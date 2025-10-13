using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Domain.Entities;
using MediatR;
using AutoTallerManager.Application.Features.Clientes.Commands;

namespace AutoTallerManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ClientesController> _logger;
    private readonly IMediator _mediator;

    public ClientesController(IUnitOfWork unitOfWork, ILogger<ClientesController> logger, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        CancellationToken ct = default)
    {
        try
        {
            var clientes = await _unitOfWork.Clientes.GetAllAsync(
                filter: c => string.IsNullOrEmpty(searchTerm) ||
                        (!string.IsNullOrEmpty(c.NombreCompleto) && c.NombreCompleto.Contains(searchTerm)) ||
                        (!string.IsNullOrEmpty(c.Email) && c.Email.Contains(searchTerm)),
                orderBy: q => q.OrderBy(c => c.NombreCompleto),
                includeProperties: "Vehiculos",
                skip: (pageNumber - 1) * pageSize,
                take: pageSize,
                ct: ct);

            var totalCount = await _unitOfWork.Clientes.CountAsync(
                filter: c => string.IsNullOrEmpty(searchTerm) ||
                        (!string.IsNullOrEmpty(c.NombreCompleto) && c.NombreCompleto.Contains(searchTerm)) ||
                        (!string.IsNullOrEmpty(c.Email) && c.Email.Contains(searchTerm)),
                ct: ct);

            Response.Headers["X-Total-Count"] = totalCount.ToString();
            Response.Headers["X-Page-Number"] = pageNumber.ToString();
            Response.Headers["X-Page-Size"] = pageSize.ToString();

            return Ok(clientes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener clientes");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Cliente>> GetCliente(int id, CancellationToken ct = default)
    {
        try
        {
            // ✅ MANTENIENDO TU ENFOQUE CON IUnitOfWork
            var cliente = await _unitOfWork.Clientes.GetByIdAsync(id, ct, new[] { "Vehiculos", "Facturas" });
            
            if (cliente == null)
                return NotFound($"Cliente con ID {id} no encontrado");

            return Ok(cliente);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener cliente {ClienteId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<ActionResult<Cliente>> CreateCliente([FromBody] CreateClienteCommand command, CancellationToken ct = default)
    {
        try
        {
            var clienteId = await _mediator.Send(command, ct);
            _logger.LogInformation("Cliente creado con ID {ClienteId}", clienteId);

            return CreatedAtAction(nameof(GetCliente), new { id = clienteId }, new { Id = clienteId });
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

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<ActionResult<Cliente>> UpdateCliente(int id, Cliente cliente, CancellationToken ct = default)
    {
        try
        {
            if (id != cliente.Id)
                return BadRequest("El ID del cliente no coincide");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingCliente = await _unitOfWork.Clientes.GetByIdAsync(id, ct);
            if (existingCliente == null)
                return NotFound($"Cliente con ID {id} no encontrado");

            // ✅ VALIDACIÓN DE EMAIL ÚNICO (similar al ejemplo)
            var emailExists = await _unitOfWork.Clientes.ExistsAsync(
                c => c.Email == cliente.Email && c.Id != id, ct);
            if (emailExists)
                return BadRequest("Ya existe otro cliente con este email");

            existingCliente.NombreCompleto = cliente.NombreCompleto;
            existingCliente.Email = cliente.Email;
            existingCliente.Telefono = cliente.Telefono;

            await _unitOfWork.Clientes.UpdateAsync(existingCliente, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Cliente actualizado: {ClienteId} - {ClienteNombre}", id, cliente.NombreCompleto);

            return Ok(existingCliente);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar cliente {ClienteId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteCliente(int id, CancellationToken ct = default)
    {
        try
        {
            var cliente = await _unitOfWork.Clientes.GetByIdAsync(id, ct, new[] { "Vehiculos.OrdenesServicio" });
            if (cliente == null)
                return NotFound($"Cliente con ID {id} no encontrado");

            var hasActiveOrders = cliente.Vehiculos?.Any(v => 
                v.OrdenesServicio?.Any(o => 
                    (o.Estado?.NombreEstServ != "Completada") && 
                    (o.Estado?.NombreEstServ != "Cancelada")) ?? false
                ) ?? false;

            if (hasActiveOrders)
                return BadRequest("No se puede eliminar el cliente porque tiene órdenes de servicio activas");

            await _unitOfWork.Clientes.DeleteAsync(id, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Cliente eliminado: {ClienteId} - {ClienteNombre}", id, cliente.NombreCompleto);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar cliente {ClienteId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPost("registrar-con-vehiculo")]
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<ActionResult<int>> RegistrarClienteConVehiculo(
        [FromBody] RegistrarClienteConVehiculoCommand command,
        CancellationToken ct = default)
    {
        try
        {
            var clienteId = await _mediator.Send(command, ct);
            
            _logger.LogInformation("Cliente registrado con vehículos: {ClienteId}", clienteId);
            
            return CreatedAtAction(nameof(GetCliente), new { id = clienteId }, new { clienteId });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error de validación al registrar cliente con vehículo");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar cliente con vehículo");
            return StatusCode(500, "Error interno del servidor");
        }
    }
}