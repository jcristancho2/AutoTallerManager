using AutoTallerManager.Domain.Entities;
using System.Linq.Expressions;

namespace AutoTallerManager.Application.Abstractions.Interfaces;

public interface IOrdenServicioService
{
    Task<OrdenServicio?> GetByIdAsync(int id, CancellationToken ct = default, params string[] includeProperties);
    Task<IEnumerable<OrdenServicio>> GetAllAsync(
        Expression<Func<OrdenServicio, bool>>? filter = null,
        Func<IQueryable<OrdenServicio>, IOrderedQueryable<OrdenServicio>>? orderBy = null,
        string includeProperties = "",
        int? skip = null,
        int? take = null,
        CancellationToken ct = default);
    Task<int> CountAsync(Expression<Func<OrdenServicio, bool>>? filter = null, CancellationToken ct = default);
    Task<bool> ExistsAsync(Expression<Func<OrdenServicio, bool>> filter, CancellationToken ct = default);
    Task AddAsync(OrdenServicio ordenServicio, CancellationToken ct = default);
    Task UpdateAsync(OrdenServicio ordenServicio, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}