using System;
using Microsoft.AspNetCore.RateLimiting;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoTallerManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[EnableRateLimiting("Facturas")]
public class InvoicesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<InvoicesController> _logger;

    public InvoicesController(IUnitOfWork unitOfWork, ILogger<InvoicesController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoices(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? customerId = null,
        [FromQuery] int? serviceOrderId = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        CancellationToken ct = default)
    {
        try
        {
            Expression<Func<Invoice, bool>>? filter = f =>
                (!customerId.HasValue || f.CustomerId == customerId.Value) &&
                (!serviceOrderId.HasValue || f.ServiceOrderId == serviceOrderId.Value) &&
                (!dateFrom.HasValue || f.InvoiceDate >= dateFrom.Value) &&
                (!dateTo.HasValue || f.InvoiceDate <= dateTo.Value);

            var invoices = await _unitOfWork.Invoices.GetAllAsync(
                filter: filter,
                orderBy: q => q.OrderByDescending(f => f.InvoiceDate),
                includeProperties: "Customer,ServiceOrder,PaymentType",
                skip: (pageNumber - 1) * pageSize,
                take: pageSize,
                ct: ct);

            var total = await _unitOfWork.Invoices.CountAsync(filter, ct);
            Response.Headers["X-Total-Count"] = total.ToString();
            Response.Headers["X-Page-Number"] = pageNumber.ToString();
            Response.Headers["X-Page-Size"] = pageSize.ToString();

            return Ok(invoices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener facturas");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Invoice>> GetInvoice(int id, CancellationToken ct = default)
    {
        try
        {
            var invoice = await _unitOfWork.Invoices.GetByIdAsync(id, ct, "Customer", "ServiceOrder", "PaymentType");
            if (invoice == null)
                return NotFound("Invoice with ID {id} not found");

            return Ok(invoice);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invoice {InvoiceId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("cliente/{clienteId}")]
    public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoicesByCustomer(int customerId, CancellationToken ct = default)
    {
        try
        {
            var invoices = await _unitOfWork.Invoices.GetFacturasByClienteAsync(customerId, ct);
            return Ok(invoices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invoices for customer {CustomerId}", customerId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("ingresos")]
    public async Task<ActionResult<decimal>> GetRevenue([FromQuery] DateTime dateFrom, [FromQuery] DateTime dateTo, CancellationToken ct = default)
    {
        try
        {
            var total = await _unitOfWork.Invoices.GetTotalIngresosAsync(dateFrom, dateTo, ct);
            return Ok(total);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating revenue between {From} and {To}", dateFrom, dateTo);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<ActionResult<Invoice>> CreateInvoice(Invoice invoice, CancellationToken ct = default)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = await _unitOfWork.ServiceOrders.GetByIdAsync(invoice.ServiceOrderId, ct);
            if (order == null)
                return BadRequest("Service order does not exist");

            var customer = await _unitOfWork.Customer.GetByIdAsync(invoice.CustomerId, ct);
            if (customer == null)
                return BadRequest("Customer does not exist");

            await _unitOfWork.Invoices.AddAsync(invoice, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Invoice created: {InvoiceId}", invoice.Id);
            return CreatedAtAction(nameof(GetInvoice), new { id = invoice.Id }, invoice);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating invoice");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<ActionResult<Invoice>> UpdateInvoice(int id, Invoice invoice, CancellationToken ct = default)
    {
        try
        {
            if (id != invoice.Id)
                return BadRequest("El ID de la factura no coincide");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _unitOfWork.Invoices.GetByIdAsync(id, ct);
            if (existing == null)
                return NotFound($"Factura con ID {id} no encontrada");

            // Validar relaciones actualizadas
            var orden = await _unitOfWork.ServiceOrders.GetByIdAsync(invoice.ServiceOrderId, ct);
            if (orden == null)
                return BadRequest("La orden de servicio especificada no existe");

            var cliente = await _unitOfWork.Customer.GetByIdAsync(invoice.CustomerId, ct);
            if (cliente == null)
                return BadRequest("El cliente especificado no existe");

            existing.InvoiceDate = invoice.InvoiceDate;
            existing.Total = invoice.Total;
            existing.ServiceOrderId = invoice.ServiceOrderId;
            existing.CustomerId = invoice.CustomerId;
            existing.PaymentTypeId = invoice.PaymentTypeId;

            await _unitOfWork.Invoices.UpdateAsync(existing, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Factura actualizada: {FacturaId}", id);
            return Ok(existing);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar factura {FacturaId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteInvoice(int id, CancellationToken ct = default)
    {
        try
        {
            var deleted = await _unitOfWork.Invoices.DeleteAsync(id, ct);
            if (!deleted)
                return NotFound($"Invoice with ID {id} not found");

            await _unitOfWork.SaveChangesAsync(ct);
            _logger.LogInformation("Invoice deleted: {InvoiceId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting invoice {InvoiceId}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}