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

    /// <summary>
    /// Gets all clients with pagination
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="searchTerm">Search term for name or email</param>
    /// <returns>Paginated list of clients</returns>
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
                        c.NombreCompleto.Contains(searchTerm) || 
                        c.Correo.Contains(searchTerm),
                orderBy: q => q.OrderBy(c => c.NombreCompleto),
                includeProperties: "Vehiculos",
                skip: (pageNumber - 1) * pageSize,
                take: pageSize,
                ct: ct);

            var totalCount = await _unitOfWork.Clientes.CountAsync(
                filter: c => string.IsNullOrEmpty(searchTerm) || 
                        c.NombreCompleto.Contains(searchTerm) || 
                        c.Correo.Contains(searchTerm),
                ct: ct);

            Response.Headers.Add("X-Total-Count", totalCount.ToString());
            Response.Headers.Add("X-Page-Number", pageNumber.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());

            return Ok(clientes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener clientes");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Gets a client by ID
    /// </summary>
    /// <param name="id">Client ID</param>
    /// <returns>Found client</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Cliente>> GetCliente(Guid id, CancellationToken ct = default)
    {
        try
        {
            var cliente = await _unitOfWork.Clientes.GetByIdAsync(id, ct, "Vehiculos", "Facturas");
            
            if (cliente == null)
            {
                return NotFound($"Cliente con ID {id} no encontrado");
            }

            return Ok(cliente);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener cliente {ClienteId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Creates a new client
    /// </summary>
    /// <param name="cliente">Client data to create</param>
    /// <returns>Created client</returns>
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
    /// Updates an existing client
    /// </summary>
    /// <param name="id">Client ID</param>
    /// <param name="cliente">Updated client data</param>
    /// <returns>Updated client</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<ActionResult<Cliente>> UpdateCliente(Guid id, Cliente cliente, CancellationToken ct = default)
    {
        try
        {
            if (id != cliente.Id)
            {
                return BadRequest("El ID del cliente no coincide");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingCliente = await _unitOfWork.Clientes.GetByIdAsync(id, ct);
            if (existingCliente == null)
            {
                return NotFound($"Cliente con ID {id} no encontrado");
            }

            // Validate unique email (excluding current client)
            var emailExists = await _unitOfWork.Clientes.ExistsAsync(
                c => c.Correo == cliente.Correo && c.Id != id, ct);
            if (emailExists)
            {
                return BadRequest("Ya existe otro cliente con este email");
            }

            // Update fields
            existingCliente.UpdateInfo(cliente.NombreCompleto, cliente.Telefono, cliente.Correo);

            _unitOfWork.Clientes.Update(existingCliente);
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

    /// <summary>
    /// Deletes a client
    /// </summary>
    /// <param name="id">Client ID</param>
    /// <returns>Operation result</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteCliente(Guid id, CancellationToken ct = default)
    {
        try
        {
            var cliente = await _unitOfWork.Clientes.GetByIdAsync(id, ct, "Vehiculos.OrdenesServicio");
            if (cliente == null)
            {
                return NotFound($"Cliente con ID {id} no encontrado");
            }

            // Check if there are active service orders
            var hasActiveOrders = cliente.Vehiculos.Any(v => 
                v.OrdenesServicio.Any(o => o.Estado?.NombreEstServ != "Completada" && o.Estado?.NombreEstServ != "Cancelada"));

            if (hasActiveOrders)
            {
                return BadRequest("No se puede eliminar el cliente porque tiene órdenes de servicio activas");
            }

            _unitOfWork.Clientes.Delete(cliente);
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