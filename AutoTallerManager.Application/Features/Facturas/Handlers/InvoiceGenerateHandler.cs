using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Features.Facturas.Commands;
using AutoTallerManager.Domain.Entities;
using MediatR;

namespace AutoTallerManager.Application.Features.Facturas.Handlers;

public sealed class InvoiceGenerateHandler : IRequestHandler<InvoiceGenerateCommand, InvoiceGenerateResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public InvoiceGenerateHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<InvoiceGenerateResponse> Handle(InvoiceGenerateCommand request, CancellationToken ct)
    {
        // Obtener la orden de servicio con todos sus detalles
        var order = await _unitOfWork.ServiceOrders.GetByIdAsync(
            request.ServiceOrderId, 
            ct, 
            "DetallesOrden", 
            "Vehiculo", 
            "Cliente", 
            "Mecanico");

        if (order == null)
        {
            throw new KeyNotFoundException($"Orden de servicio con ID {request.ServiceOrderId} no encontrada.");
        }

        // Verificar que la orden esté en estado "Completada"
        if (order.StatusId != 3) // Estado "Completada"
        {
            throw new InvalidOperationException("Solo se pueden generar facturas para órdenes completadas.");
        }

        // Verificar que no exista ya una factura para esta orden
        var invoiceExists = await _unitOfWork.Invoices.GetByOrderServiceIdAsync(request.ServiceOrderId, ct);
        if (invoiceExists != null)
        {
            throw new InvalidOperationException("Ya existe una factura para esta orden de servicio.");
        }

        // Calcular totales
        decimal SubtotalSpare = 0;
        decimal SubtotalLabor = 0;

        if (order.OrderDetails != null)
        {
            foreach (var detail in order.OrderDetails)
            {
                SubtotalSpare += detail.Quantity * detail.UnitPrice;
                SubtotalLabor += detail.LaborCost;
            }
        }

        decimal total = SubtotalSpare + SubtotalLabor;

        // Generar número de factura único
        var invoiceNumber = await GenerateInvoiceNumberAsync(ct);

        // Crear la factura
        var invoice = new Invoice
        {
            ServiceOrderId = order.Id,
            CustomerId = order.Vehicle?.CustomerId ?? throw new InvalidOperationException("No se pudo obtener el cliente de la orden."),
            InvoiceDate = DateTime.UtcNow,
            Total = total,
            PaymentTypeId = request.PaymentTypeId,
            InvoiceNumber = invoiceNumber,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Invoices.AddAsync(invoice, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new InvoiceGenerateResponse
        {
            InvoiceId = invoice.Id,
            Total = total,
            SubtotalSpare = SubtotalSpare,
            SubtotalLabor = SubtotalLabor,
            InvoiceDate = invoice.InvoiceDate,
            InvoiceNumber = invoice.InvoiceNumber
        };
    }

    private async Task<string> GenerateInvoiceNumberAsync(CancellationToken ct)
    {
        // Obtener el último número de factura
        var LastInvoice = await _unitOfWork.Invoices.GetLastInvoiceAsync(ct);
        
        int siguienteNumero = 1;
        if (LastInvoice != null && !string.IsNullOrEmpty(LastInvoice.InvoiceNumber))
        {
            // Extraer el número de la última factura (formato: FAC-YYYY-NNNNNN)
            var partes = LastInvoice.InvoiceNumber.Split('-');
            if (partes.Length == 3 && int.TryParse(partes[2], out int ultimoNumero))
            {
                siguienteNumero = ultimoNumero + 1;
            }
        }

        // Formato: FAC-YYYY-NNNNNN
        var año = DateTime.UtcNow.Year;
        return $"FAC-{año}-{siguienteNumero:D6}";
    }
}


