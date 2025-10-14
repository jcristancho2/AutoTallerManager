using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Features.Facturas.Commands;
using AutoTallerManager.Domain.Entities;
using MediatR;

namespace AutoTallerManager.Application.Features.Facturas.Handlers;

public sealed class GenerarFacturaHandler : IRequestHandler<GenerarFacturaCommand, GenerarFacturaResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GenerarFacturaHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GenerarFacturaResponse> Handle(GenerarFacturaCommand request, CancellationToken ct)
    {
        // Obtener la orden de servicio con todos sus detalles
        var orden = await _unitOfWork.OrdenesServicio.GetByIdAsync(
            request.OrdenServicioId, 
            ct, 
            "DetallesOrden", 
            "Vehiculo", 
            "Cliente", 
            "Mecanico");

        if (orden == null)
        {
            throw new KeyNotFoundException($"Orden de servicio con ID {request.OrdenServicioId} no encontrada.");
        }

        // Verificar que la orden esté en estado "Completada"
        if (orden.EstadoId != 3) // Estado "Completada"
        {
            throw new InvalidOperationException("Solo se pueden generar facturas para órdenes completadas.");
        }

        // Verificar que no exista ya una factura para esta orden
        var facturaExistente = await _unitOfWork.Facturas.GetByOrdenServicioIdAsync(request.OrdenServicioId, ct);
        if (facturaExistente != null)
        {
            throw new InvalidOperationException("Ya existe una factura para esta orden de servicio.");
        }

        // Calcular totales
        decimal subtotalRepuestos = 0;
        decimal subtotalManoDeObra = 0;

        if (orden.DetallesOrden != null)
        {
            foreach (var detalle in orden.DetallesOrden)
            {
                subtotalRepuestos += detalle.Cantidad * detalle.PrecioUnitario;
                subtotalManoDeObra += detalle.PrecioManoDeObra;
            }
        }

        decimal total = subtotalRepuestos + subtotalManoDeObra;

        // Generar número de factura único
        var numeroFactura = await GenerarNumeroFacturaAsync(ct);

        // Crear la factura
        var factura = new Factura
        {
            OrdenServicioId = orden.Id,
            ClienteId = orden.Vehiculo?.ClienteId ?? throw new InvalidOperationException("No se pudo obtener el cliente de la orden."),
            Fecha = DateTime.UtcNow,
            Total = total,
            TipoPagoId = request.TipoPagoId,
            NumeroFactura = numeroFactura,
            Observaciones = request.Observaciones,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Facturas.AddAsync(factura, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new GenerarFacturaResponse
        {
            FacturaId = factura.Id,
            Total = total,
            SubtotalRepuestos = subtotalRepuestos,
            SubtotalManoDeObra = subtotalManoDeObra,
            FechaGeneracion = factura.Fecha,
            NumeroFactura = numeroFactura
        };
    }

    private async Task<string> GenerarNumeroFacturaAsync(CancellationToken ct)
    {
        // Obtener el último número de factura
        var ultimaFactura = await _unitOfWork.Facturas.GetUltimaFacturaAsync(ct);
        
        int siguienteNumero = 1;
        if (ultimaFactura != null && !string.IsNullOrEmpty(ultimaFactura.NumeroFactura))
        {
            // Extraer el número de la última factura (formato: FAC-YYYY-NNNNNN)
            var partes = ultimaFactura.NumeroFactura.Split('-');
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


