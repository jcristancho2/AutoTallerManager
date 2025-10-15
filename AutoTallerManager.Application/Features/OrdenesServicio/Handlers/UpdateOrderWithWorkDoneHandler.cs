using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Features.OrdenesServicio.Commands;
using AutoTallerManager.Domain.Entities;
using MediatR;

namespace AutoTallerManager.Application.Features.OrdenesServicio.Handlers;

public sealed class UpdateOrderWithWorkDoneHandler : IRequestHandler<UpdateOrderWithWorkDoneCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrderWithWorkDoneHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateOrderWithWorkDoneCommand request, CancellationToken ct)
    {
        // Obtener la orden con sus detalles
        var order = await _unitOfWork.ServiceOrders.GetByIdAsync(request.OrderId, ct, "DetallesOrden", "Vehiculo", "Mecanico");
        if (order == null)
        {
            throw new KeyNotFoundException($"Orden de servicio con ID {request.OrderId} no encontrada.");
        }

        // Validar que la orden esté en un estado válido para actualizar
        if (order.StatusId == 4) // Estado "Cancelada"
        {
            throw new InvalidOperationException("No se puede actualizar una orden cancelada.");
        }

        // Procesar repuestos utilizados
        foreach (var UsedSpare in request.UsedSpare)
        {
            // Verificar que el repuesto existe y tiene stock suficiente
            var spare = await _unitOfWork.Spares.GetByIdAsync(UsedSpare.SpareId, ct);
            if (spare == null)
            {
                throw new KeyNotFoundException($"Repuesto con ID {UsedSpare.SpareId} no encontrado.");
            }

            if (spare.Stock < UsedSpare.Quantity)
            {
                throw new InvalidOperationException($"Stock insuficiente para el repuesto {spare.Name}. Stock disponible: {spare.Stock}, Cantidad requerida: {UsedSpare.Quantity}");
            }

            // Crear o actualizar detalle de orden
            var existingDetail = order.OrderDetails?.FirstOrDefault(d => d.SpareId == UsedSpare.SpareId);

            if (existingDetail != null)
            {
                // Actualizar detalle existente
                existingDetail.Quantity += UsedSpare.Quantity;
                existingDetail.UnitPrice = UsedSpare.UnitPrice;
                existingDetail.Description = UsedSpare.Description;
                existingDetail.LaborCost = request.LaborCost;
                existingDetail.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.OrderDetails.UpdateAsync(existingDetail, ct);
            }
            else
            {
                // Crear nuevo detalle
                var newDetail = new OrderDetail
                {
                    ServiceOrderId = order.Id,
                    SpareId = UsedSpare.SpareId,
                    Quantity = UsedSpare.Quantity,
                    UnitPrice = UsedSpare.UnitPrice,
                    LaborCost = request.LaborCost,
                    Description = UsedSpare.Description,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.OrderDetails.AddAsync(newDetail, ct);
            }

            // Descontar stock del repuesto
            spare.Stock -= UsedSpare.Quantity;
            spare.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Spares.UpdateAsync(spare, ct);
        }

        // Actualizar la orden
        // order.Description = request.Description;
        order.UpdatedAt = DateTime.UtcNow;

        // Cambiar estado si se especifica
        if (request.NewStatusId.HasValue)
        {
            order.StatusId = request.NewStatusId.Value;
        }

        await _unitOfWork.ServiceOrders.UpdateAsync(order, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return true;
    }
}


