using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Features.OrdenesServicio.Commands;
using AutoTallerManager.Domain.Entities;
using MediatR;

namespace AutoTallerManager.Application.Features.OrdenesServicio.Handlers;

public sealed class AssignSpareHandler : IRequestHandler<AssignSpareCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public AssignSpareHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(AssignSpareCommand request, CancellationToken ct)
    {
        var order = await _unitOfWork.ServiceOrders.GetByIdAsync(request.OrderId, ct, "OrderDetails");
        if (order is null)
            throw new KeyNotFoundException("Orden de servicio no encontrada");

        foreach (var item in request.Spare)
        {
            var spare = await _unitOfWork.Spares.GetByIdAsync(item.SpareId, ct);
            if (spare is null)
                throw new KeyNotFoundException($"Repuesto {item.SpareId} no encontrado");

            if (spare.Stock < item.Quantity)
                throw new InvalidOperationException($"Stock insuficiente para el repuesto {spare.Name} (id={spare.Id})");

            // Crear detalle
            var detail = new OrderDetail
            {
                ServiceOrderId = order.Id,
                SpareId = spare.Id,
                Quantity = item.Quantity,
                UnitPrice = spare.UnitPrice,
                LaborCost = 0m,
                Description = spare.Description
            };

            await _unitOfWork.OrderDetails.AddAsync(detail, ct);

            // Actualizar stock
            spare.Stock -= item.Quantity;
            spare.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Spares.UpdateAsync(spare, ct);
        }

        order.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.ServiceOrders.UpdateAsync(order, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
