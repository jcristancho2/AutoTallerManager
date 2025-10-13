using AutoTallerManager.Domain.Entities;
using System.Linq.Expressions;

namespace AutoTallerManager.Application.Abstractions.Interfaces;

public interface IFacturaService
{
    Task<Factura?> GetByIdAsync(int id, CancellationToken ct = default, params string[] includeProperties);
    Task<IEnumerable<Factura>> GetAllAsync(
        Expression<Func<Factura, bool>>? filter = null,
        Func<IQueryable<Factura>, IOrderedQueryable<Factura>>? orderBy = null,
        string includeProperties = "",
        int? skip = null,
        int? take = null,
        CancellationToken ct = default);
    Task<int> CountAsync(Expression<Func<Factura, bool>>? filter = null, CancellationToken ct = default);
    Task<bool> ExistsAsync(Expression<Func<Factura, bool>> filter, CancellationToken ct = default);
    Task AddAsync(Factura factura, CancellationToken ct = default);
    Task UpdateAsync(Factura factura, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Factura>> GetFacturasByClienteAsync(int clienteId, CancellationToken ct = default);
    Task<decimal> GetTotalIngresosAsync(DateTime fechaDesde, DateTime fechaHasta, CancellationToken ct = default);
    Task<Factura?> GetByOrdenServicioIdAsync(int ordenServicioId, CancellationToken ct = default);
    Task<Factura?> GetUltimaFacturaAsync(CancellationToken ct = default);
}