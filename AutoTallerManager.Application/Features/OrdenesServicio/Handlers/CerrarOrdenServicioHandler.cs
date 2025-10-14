using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Features.Facturas.Commands;
using AutoTallerManager.Application.Features.OrdenesServicio.Commands;
using MediatR;

namespace AutoTallerManager.Application.Features.OrdenesServicio.Handlers;

public sealed class CerrarOrdenServicioHandler : IRequestHandler<CerrarOrdenServicioCommand, CerrarOrdenServicioResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public CerrarOrdenServicioHandler(IUnitOfWork unitOfWork, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<CerrarOrdenServicioResponse> Handle(CerrarOrdenServicioCommand request, CancellationToken ct)
    {
        // Obtener la orden de servicio
        var orden = await _unitOfWork.OrdenesServicio.GetByIdAsync(
            request.OrdenId, 
            ct, 
            "DetallesOrden", 
            "Vehiculo", 
            "Cliente", 
            "Estado");

        if (orden == null)
        {
            throw new KeyNotFoundException($"Orden de servicio con ID {request.OrdenId} no encontrada.");
        }

        // Validar que la orden esté en estado válido para cerrar
        if (orden.EstadoId == 4) // Estado "Cancelada"
        {
            throw new InvalidOperationException("No se puede cerrar una orden cancelada.");
        }

        if (orden.EstadoId == 3) // Estado "Completada"
        {
            throw new InvalidOperationException("La orden ya está cerrada.");
        }

        // Validar que la orden tenga detalles (repuestos o trabajo realizado)
        if (orden.DetallesOrden == null || !orden.DetallesOrden.Any())
        {
            throw new InvalidOperationException("No se puede cerrar una orden sin trabajo realizado o repuestos utilizados.");
        }

        // Cambiar el estado de la orden a "Completada"
        orden.EstadoId = 3; // Estado "Completada"
        orden.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.OrdenesServicio.UpdateAsync(orden, ct);

        // Generar la factura automáticamente
        var generarFacturaCommand = new GenerarFacturaCommand
        {
            OrdenServicioId = orden.Id,
            TipoPagoId = request.TipoPagoId,
            Observaciones = request.ObservacionesFactura
        };

        var facturaResponse = await _mediator.Send(generarFacturaCommand, ct);

        // Guardar todos los cambios
        await _unitOfWork.SaveChangesAsync(ct);

        return new CerrarOrdenServicioResponse
        {
            OrdenId = orden.Id,
            FacturaId = facturaResponse.FacturaId,
            TotalFactura = facturaResponse.Total,
            NumeroFactura = facturaResponse.NumeroFactura,
            FechaCierre = DateTime.UtcNow
        };
    }
}


