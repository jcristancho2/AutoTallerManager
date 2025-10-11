using AutoTallerManager.Domain.Entities;
using System.Linq.Expressions;

namespace AutoTallerManager.Application.Abstractions.Interfaces;

public interface IDetalleOrdenService
{
    Task<DetalleOrden?> GetByIdAsync(int detalleOrdenId, int ordenServicioId, CancellationToken ct = default);
    Task<IEnumerable<DetalleOrden>> GetAllAsync(
        Expression<Func<DetalleOrden, bool>>? filter = null,
        Func<IQueryable<DetalleOrden>, IOrderedQueryable<DetalleOrden>>? orderBy = null,
        string includeProperties = "",
        int? skip = null,
        int? take = null,
        CancellationToken ct = default);
    Task<int> CountAsync(Expression<Func<DetalleOrden, bool>>? filter = null, CancellationToken ct = default);
    Task<bool> ExistsAsync(Expression<Func<DetalleOrden, bool>> filter, CancellationToken ct = default);
    Task AddAsync(DetalleOrden detalleOrden, CancellationToken ct = default);
    Task UpdateAsync(DetalleOrden detalleOrden, CancellationToken ct = default);
    Task<bool> DeleteAsync(int detalleOrdenId, int ordenServicioId, CancellationToken ct = default);
    Task<IEnumerable<DetalleOrden>> GetDetallesByOrdenAsync(int ordenServicioId, CancellationToken ct = default);
}


