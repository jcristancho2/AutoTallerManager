using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Features.OrdenesServicio.Commands;
using AutoTallerManager.Domain.Entities;
using MediatR;

namespace AutoTallerManager.Application.Features.OrdenesServicio.Handlers;

public sealed class AsignarRepuestosHandler : IRequestHandler<AsignarRepuestosCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public AsignarRepuestosHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(AsignarRepuestosCommand request, CancellationToken ct)
    {
        var orden = await _unitOfWork.OrdenesServicio.GetByIdAsync(request.OrdenId, ct, "DetallesOrden");
        if (orden is null)
            throw new KeyNotFoundException("Orden de servicio no encontrada");

        foreach (var item in request.Repuestos)
        {
            var repuesto = await _unitOfWork.Repuestos.GetByIdAsync(item.RepuestoId, ct);
            if (repuesto is null)
                throw new KeyNotFoundException($"Repuesto {item.RepuestoId} no encontrado");

            if (repuesto.Stock < item.Cantidad)
                throw new InvalidOperationException($"Stock insuficiente para el repuesto {repuesto.NombreRepu} (id={repuesto.Id})");

            // Crear detalle
            var detalle = new DetalleOrden
            {
                OrdenServicioId = orden.Id,
                RepuestoId = repuesto.Id,
                Cantidad = item.Cantidad,
                PrecioUnitario = repuesto.PrecioUnitario,
                PrecioManoDeObra = 0m,
                Descripcion = repuesto.Descripcion
            };

            await _unitOfWork.DetallesOrden.AddAsync(detalle, ct);

            // Actualizar stock
            repuesto.Stock -= item.Cantidad;
            repuesto.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Repuestos.UpdateAsync(repuesto, ct);
        }

        orden.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.OrdenesServicio.UpdateAsync(orden, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
