using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Application.Services;

public interface IVehicleServiceAvailabilityValidator
{
    Task<bool> IsVehicleAvailableAsync(int vehicleId, DateTime startDate, DateTime? endDate = null, int? excludeOrderId = null, CancellationToken ct = default);
    Task<IEnumerable<ServiceOrder>> GetActiveOrdersForVehicleAsync(int vehicleId, CancellationToken ct = default);
}

public class VehicleServiceAvailabilityValidator : IVehicleServiceAvailabilityValidator
{
    private readonly IUnitOfWork _unitOfWork;

    public VehicleServiceAvailabilityValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> IsVehicleAvailableAsync(int vehicleId, DateTime startDate, DateTime? endDate = null, int? excludeOrderId = null, CancellationToken ct = default)
    {
        // Obtener todas las órdenes activas del vehículo
        var activeOrders = await GetActiveOrdersForVehicleAsync(vehicleId, ct);
        
        // Filtrar la orden que estamos excluyendo (para actualizaciones)
        if (excludeOrderId.HasValue)
        {
            activeOrders = activeOrders.Where(o => o.Id != excludeOrderId.Value);
        }

        // Verificar si hay conflictos de fechas
        foreach (var order in activeOrders)
        {
            var endDateOrder = order.EstimatedDeliveryDate;

            // Verificar solapamiento de fechas
            if (FechaSolapada(startDate, endDate ?? startDate.AddDays(1), order.EntryDate, endDateOrder))
            {
                return false;
            }
        }

        return true;
    }

    public async Task<IEnumerable<ServiceOrder>> GetActiveOrdersForVehicleAsync(int vehicleId, CancellationToken ct = default)
    {
        // Estados activos: Pendiente (1), En Proceso (2), Esperando Repuestos (5)
        var StatusActives = new[] { 1, 2, 5 };

        return await _unitOfWork.ServiceOrders.GetAllAsync(
            filter: o => o.VehicleId == vehicleId && StatusActives.Contains(o.StatusId),
            orderBy: q => q.OrderBy(o => o.EntryDate),
            includeProperties: "Estado",
            ct: ct);
    }

    private static bool FechaSolapada(DateTime inicio1, DateTime fin1, DateTime inicio2, DateTime fin2)
    {
        return inicio1 <= fin2 && fin1 >= inicio2;
    }
}


