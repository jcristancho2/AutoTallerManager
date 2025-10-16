using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Features.Customers.Commands;
using MediatR;

namespace AutoTallerManager.Application.Features.Customers.Handlers;

public sealed class DeleteCustomerHandler : IRequestHandler<DeleteCustomerCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCustomerHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteCustomerCommand request, CancellationToken ct)
    {
        // Obtener el cliente existente
        var customerExists = await _unitOfWork.Customer.GetByIdAsync(request.Id, ct, new[] { "Vehicles" });
        if (customerExists == null)
        {
            throw new KeyNotFoundException($"Cliente con ID {request.Id} no encontrado.");
        }

        // Verificar si el cliente tiene vehículos con órdenes de servicio activas
        if (customerExists.Vehicles != null && customerExists.Vehicles.Any())
        {
            foreach (var vehicle in customerExists.Vehicles)
            {
                var activeOrders = await _unitOfWork.ServiceOrders.GetAllAsync(
                    filter: o => o.VehicleId == vehicle.Id && o.StatusId != 3 && o.StatusId != 4, // No completada ni cancelada
                    ct: ct);

                if (activeOrders.Any())
                {
                    throw new InvalidOperationException($"No se puede eliminar el cliente porque tiene vehículos con órdenes de servicio activas.");
                }
            }
        }

        // Eliminar el cliente
        await _unitOfWork.Customer.DeleteAsync(request.Id, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return true;
    }
}


