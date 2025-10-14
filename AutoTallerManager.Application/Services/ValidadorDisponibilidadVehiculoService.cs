using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Application.Services;

public interface IValidadorDisponibilidadVehiculoService
{
    Task<bool> VehiculoDisponibleAsync(int vehiculoId, DateTime fechaInicio, DateTime? fechaFin = null, int? ordenExcluir = null, CancellationToken ct = default);
    Task<IEnumerable<OrdenServicio>> GetOrdenesActivasVehiculoAsync(int vehiculoId, CancellationToken ct = default);
}

public class ValidadorDisponibilidadVehiculoService : IValidadorDisponibilidadVehiculoService
{
    private readonly IUnitOfWork _unitOfWork;

    public ValidadorDisponibilidadVehiculoService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> VehiculoDisponibleAsync(int vehiculoId, DateTime fechaInicio, DateTime? fechaFin = null, int? ordenExcluir = null, CancellationToken ct = default)
    {
        // Obtener todas las órdenes activas del vehículo
        var ordenesActivas = await GetOrdenesActivasVehiculoAsync(vehiculoId, ct);
        
        // Filtrar la orden que estamos excluyendo (para actualizaciones)
        if (ordenExcluir.HasValue)
        {
            ordenesActivas = ordenesActivas.Where(o => o.Id != ordenExcluir.Value);
        }

        // Verificar si hay conflictos de fechas
        foreach (var orden in ordenesActivas)
        {
            var fechaFinOrden = orden.FechaEstimadaEntrega;
            
            // Verificar solapamiento de fechas
            if (FechaSolapada(fechaInicio, fechaFin ?? fechaInicio.AddDays(1), orden.FechaIngreso, fechaFinOrden))
            {
                return false;
            }
        }

        return true;
    }

    public async Task<IEnumerable<OrdenServicio>> GetOrdenesActivasVehiculoAsync(int vehiculoId, CancellationToken ct = default)
    {
        // Estados activos: Pendiente (1), En Proceso (2), Esperando Repuestos (5)
        var estadosActivos = new[] { 1, 2, 5 };
        
        return await _unitOfWork.OrdenesServicio.GetAllAsync(
            filter: o => o.VehiculoId == vehiculoId && estadosActivos.Contains(o.EstadoId),
            orderBy: q => q.OrderBy(o => o.FechaIngreso),
            includeProperties: "Estado",
            ct: ct);
    }

    private static bool FechaSolapada(DateTime inicio1, DateTime fin1, DateTime inicio2, DateTime fin2)
    {
        return inicio1 <= fin2 && fin1 >= inicio2;
    }
}


