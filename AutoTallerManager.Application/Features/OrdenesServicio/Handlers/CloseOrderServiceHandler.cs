using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Features.Facturas.Commands;
using AutoTallerManager.Application.Features.OrdenesServicio.Commands;
using MediatR;

namespace AutoTallerManager.Application.Features.OrdenesServicio.Handlers;

public sealed class CloseOrderServiceHandler : IRequestHandler<CloseOrderServiceCommand, CloseOrderServiceResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public CloseOrderServiceHandler(IUnitOfWork unitOfWork, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<CloseOrderServiceResponse> Handle(CloseOrderServiceCommand request, CancellationToken ct)
    {
        // Obtener la orden de servicio
        var order = await _unitOfWork.ServiceOrders.GetByIdAsync(
            request.OrderId, 
            ct, 
            "DetallesOrden", 
            "Vehiculo", 
            "Cliente", 
            "Estado");

        if (order == null)
        {
            throw new KeyNotFoundException($"Orden de servicio con ID {request.OrderId} no encontrada.");
        }

        // Validar que la orden esté en estado válido para cerrar
        if (order.StatusId == 4) // Estado "Cancelada"
        {
            throw new InvalidOperationException("No se puede cerrar una orden cancelada.");
        }

        if (order.StatusId == 3) // Estado "Completada"
        {
            throw new InvalidOperationException("La orden ya está cerrada.");
        }

        // Validar que la orden tenga detalles (repuestos o trabajo realizado)
        if (order.OrderDetails == null || !order.OrderDetails.Any())
        {
            throw new InvalidOperationException("No se puede cerrar una orden sin trabajo realizado o repuestos utilizados.");
        }

        // Cambiar el estado de la orden a "Completada"
        order.StatusId = 3; // Estado "Completada"
        order.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.ServiceOrders.UpdateAsync(order, ct);

        // Generar la factura automáticamente
        var invoiceGenerateCommand = new InvoiceGenerateCommand
        {
            ServiceOrderId = order.Id,
            PaymentTypeId = request.PaymentTypeId,
            Observations = request.InvoiceNotes
        };

        var invoiceResponse = await _mediator.Send(invoiceGenerateCommand, ct);

        // Guardar todos los cambios
        await _unitOfWork.SaveChangesAsync(ct);

        return new CloseOrderServiceResponse
        {
            OrderId = order.Id,
            InvoiceId = invoiceResponse.InvoiceId,
            TotalInvoice = invoiceResponse.Total,
            InvoiceNumber = invoiceResponse.InvoiceNumber,
            CloseDate = DateTime.UtcNow
        };
    }
}


