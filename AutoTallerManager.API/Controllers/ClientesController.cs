using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Domain.Entities;
using MediatR;
using AutoTallerManager.Application.Features.Clientes.Commands;
using AutoTallerManager.API.DTOs.Request;

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
                        (!string.IsNullOrEmpty(c.Email) && c.Email.Contains(searchTerm) || c.Email == searchTerm),
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

    /// <summary>
    /// Crear cliente con dirección completa (país, departamento, ciudad)
    /// </summary>
    /// <param name="request">Datos del cliente con dirección completa</param>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>Cliente creado con información de ubicación</returns>
    [HttpPost("completo")]
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<ActionResult<CreateClienteCompletoResponse>> CreateClienteCompleto(
        [FromBody] CreateClienteCompletoDto request, 
        CancellationToken ct = default)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new CreateClienteCompletoCommand(
                request.NombreCompleto,
                request.Telefono,
                request.Email,
                request.TipoClienteId,
                request.Direccion.Descripcion,
                request.Direccion.PaisId,
                request.Direccion.DepartamentoId,
                request.Direccion.CiudadId
            );

            var response = await _mediator.Send(command, ct);
            _logger.LogInformation("Cliente completo creado con ID {ClienteId}", response.ClienteId);

            return CreatedAtAction(nameof(GetCliente), new { id = response.ClienteId }, response);
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
    /// <returns>Cliente y vehículos creados</returns>
    [HttpPost("registrar-con-vehiculo-completo")]
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<ActionResult<object>> RegistrarClienteCompletoConVehiculo(
        [FromBody] RegistrarClienteCompletoConVehiculoDto request, 
        CancellationToken ct = default)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Primero crear el cliente completo
            var clienteCommand = new CreateClienteCompletoCommand(
                request.Cliente.NombreCompleto,
                request.Cliente.Telefono,
                request.Cliente.Email,
                request.Cliente.TipoClienteId,
                request.Cliente.Direccion.Descripcion,
                request.Cliente.Direccion.PaisId,
                request.Cliente.Direccion.DepartamentoId,
                request.Cliente.Direccion.CiudadId
            );

            var clienteResponse = await _mediator.Send(clienteCommand, ct);

            // Luego crear los vehículos si se proporcionaron
            var vehiculosCreados = new List<object>();
            if (request.Vehiculos.Any())
            {
                foreach (var vehiculoRequest in request.Vehiculos)
                {
                    // Aquí podrías implementar la lógica para crear vehículos
                    // Por ahora solo registramos que se recibieron
                    vehiculosCreados.Add(new { 
                        vin = vehiculoRequest.Vin,
                        ano = vehiculoRequest.Ano,
                        mensaje = "Vehículo pendiente de implementación"
                    });
                }
            }

            _logger.LogInformation("Cliente completo con vehículos registrado: {ClienteId}", clienteResponse.ClienteId);

            return Ok(new
            {
                cliente = clienteResponse,
                vehiculos = vehiculosCreados,
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
}