using AutoTallerManager.Domain.Entities;
using System.Linq.Expressions;

namespace AutoTallerManager.Application.Abstractions.Interfaces;

public interface IRepuestoService
{
    Task<Repuesto?> GetByIdAsync(int id, CancellationToken ct = default, params string[] includeProperties);
    Task<Repuesto?> GetByCodigoAsync(string codigo, CancellationToken ct = default);
    Task<IEnumerable<Repuesto>> GetAllAsync(
        Expression<Func<Repuesto, bool>>? filter = null,
        Func<IQueryable<Repuesto>, IOrderedQueryable<Repuesto>>? orderBy = null,
        string includeProperties = "",
        int? skip = null,
        int? take = null,
        CancellationToken ct = default);
    Task<int> CountAsync(Expression<Func<Repuesto, bool>>? filter = null, CancellationToken ct = default);
    Task<bool> ExistsAsync(Expression<Func<Repuesto, bool>> filter, CancellationToken ct = default);
    Task AddAsync(Repuesto repuesto, CancellationToken ct = default);
    Task UpdateAsync(Repuesto repuesto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Repuesto>> GetRepuestosStockBajoAsync(int stockMinimo, CancellationToken ct = default);
}