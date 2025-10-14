using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Features.OrdenesServicio.Commands;
using AutoTallerManager.Domain.Entities;
using MediatR;

namespace AutoTallerManager.Application.Features.OrdenesServicio.Handlers;

public sealed class ActualizarOrdenConTrabajoRealizadoHandler : IRequestHandler<ActualizarOrdenConTrabajoRealizadoCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarOrdenConTrabajoRealizadoHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ActualizarOrdenConTrabajoRealizadoCommand request, CancellationToken ct)
    {
        // Obtener la orden con sus detalles
        var orden = await _unitOfWork.OrdenesServicio.GetByIdAsync(request.OrdenId, ct, "DetallesOrden", "Vehiculo", "Mecanico");
        if (orden == null)
        {
            throw new KeyNotFoundException($"Orden de servicio con ID {request.OrdenId} no encontrada.");
        }

        // Validar que la orden esté en un estado válido para actualizar
        if (orden.EstadoId == 4) // Estado "Cancelada"
        {
            throw new InvalidOperationException("No se puede actualizar una orden cancelada.");
        }

        // Procesar repuestos utilizados
        foreach (var repuestoUtilizado in request.RepuestosUtilizados)
        {
            // Verificar que el repuesto existe y tiene stock suficiente
            var repuesto = await _unitOfWork.Repuestos.GetByIdAsync(repuestoUtilizado.RepuestoId, ct);
            if (repuesto == null)
            {
                throw new KeyNotFoundException($"Repuesto con ID {repuestoUtilizado.RepuestoId} no encontrado.");
            }

            if (repuesto.Stock < repuestoUtilizado.Cantidad)
            {
                throw new InvalidOperationException($"Stock insuficiente para el repuesto {repuesto.NombreRepu}. Stock disponible: {repuesto.Stock}, Cantidad requerida: {repuestoUtilizado.Cantidad}");
            }

            // Crear o actualizar detalle de orden
            var detalleExistente = orden.DetallesOrden?.FirstOrDefault(d => d.RepuestoId == repuestoUtilizado.RepuestoId);
            
            if (detalleExistente != null)
            {
                // Actualizar detalle existente
                detalleExistente.Cantidad += repuestoUtilizado.Cantidad;
                detalleExistente.PrecioUnitario = repuestoUtilizado.PrecioUnitario;
                detalleExistente.Descripcion = repuestoUtilizado.Descripcion;
                detalleExistente.PrecioManoDeObra = request.ManoDeObra;
                detalleExistente.UpdatedAt = DateTime.UtcNow;
                
                await _unitOfWork.DetallesOrden.UpdateAsync(detalleExistente, ct);
            }
            else
            {
                // Crear nuevo detalle
                var nuevoDetalle = new DetalleOrden
                {
                    OrdenServicioId = orden.Id,
                    RepuestoId = repuestoUtilizado.RepuestoId,
                    Cantidad = repuestoUtilizado.Cantidad,
                    PrecioUnitario = repuestoUtilizado.PrecioUnitario,
                    PrecioManoDeObra = request.ManoDeObra,
                    Descripcion = repuestoUtilizado.Descripcion,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.DetallesOrden.AddAsync(nuevoDetalle, ct);
            }

            // Descontar stock del repuesto
            repuesto.Stock -= repuestoUtilizado.Cantidad;
            repuesto.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Repuestos.UpdateAsync(repuesto, ct);
        }

        // Actualizar la orden
        orden.DescripcionTrabajo = request.DescripcionTrabajo;
        orden.UpdatedAt = DateTime.UtcNow;

        // Cambiar estado si se especifica
        if (request.NuevoEstadoId.HasValue)
        {
            orden.EstadoId = request.NuevoEstadoId.Value;
        }

        await _unitOfWork.OrdenesServicio.UpdateAsync(orden, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return true;
    }
}


