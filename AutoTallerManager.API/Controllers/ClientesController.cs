using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Domain.Entities;


namespace AutoTallerManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ClientesController> _logger;

    public ClientesController(IUnitOfWork unitOfWork, ILogger<ClientesController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
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
                        c.Email.Contains(searchTerm),
                orderBy: q => q.OrderBy(c => c.NombreCompleto),
                includeProperties: "Vehiculos",
                skip: (pageNumber - 1) * pageSize,
                take: pageSize,
                ct: ct);

            var totalCount = await _unitOfWork.Clientes.CountAsync(
                filter: c => string.IsNullOrEmpty(searchTerm) || 
                        c.NombreCompleto.Contains(searchTerm) || 
                        c.Email.Contains(searchTerm),
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
    public async Task<ActionResult<Cliente>> GetCliente(int id, CancellationToken ct = default)
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
    public async Task<ActionResult<Cliente>> CreateCliente(Cliente cliente, CancellationToken ct = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate unique email
            var existingCliente = await _unitOfWork.Clientes.GetByEmailAsync(cliente.Email, ct);
            if (existingCliente != null)
            {
                return BadRequest("Ya existe un cliente con este email");
            }

            cliente.FechaRegistro = DateTime.UtcNow;
            await _unitOfWork.Clientes.AddAsync(cliente, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Cliente creado: {ClienteId} - {ClienteNombre}", cliente.Id, cliente.Nombre);

            return CreatedAtAction(nameof(GetCliente), new { id = cliente.Id }, cliente);
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
    public async Task<ActionResult<Cliente>> UpdateCliente(int id, Cliente cliente, CancellationToken ct = default)
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
                c => c.Email == cliente.Email && c.Id != id, ct);
            if (emailExists)
            {
                return BadRequest("Ya existe otro cliente con este email");
            }

            // Update fields
            existingCliente.Nombre = cliente.Nombre;
            existingCliente.Email = cliente.Email;
            existingCliente.Telefono = cliente.Telefono;
            existingCliente.Direccion = cliente.Direccion;

            _unitOfWork.Clientes.Update(existingCliente);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Cliente actualizado: {ClienteId} - {ClienteNombre}", id, cliente.Nombre);

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
    public async Task<ActionResult> DeleteCliente(int id, CancellationToken ct = default)
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
                v.OrdenesServicio.Any(o => o.Estado != "Completada" && o.Estado != "Cancelada"));

            if (hasActiveOrders)
            {
                return BadRequest("No se puede eliminar el cliente porque tiene órdenes de servicio activas");
            }

            _unitOfWork.Clientes.Delete(cliente);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Cliente eliminado: {ClienteId} - {ClienteNombre}", id, cliente.Nombre);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar cliente {ClienteId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}