using AutoTallerManager.Application.Common.Models;
using AutoTallerManager.Domain.Entities;
using System.Linq.Expressions;

namespace AutoTallerManager.Application.Abstractions.Interfaces;

public interface ISpareService
{
    // Operaciones de lectura
    Task<Spare?> GetByIdAsync(int id, CancellationToken ct = default, params string[] includeProperties);
    Task<Spare?> GetByCodigoAsync(string Code, CancellationToken ct = default);
    Task<Spare?> GetByCodigoWithIncludesAsync(string Code, CancellationToken ct = default, params string[] includeProperties);

    // Operaciones de consulta múltiple
    Task<IEnumerable<Spare>> GetAllAsync(
        Expression<Func<Spare, bool>>? filter = null,
        Func<IQueryable<Spare>, IOrderedQueryable<Spare>>? orderBy = null,
        string includeProperties = "",
        int? skip = null,
        int? take = null,
        CancellationToken ct = default);
    
    // Operaciones de conteo y verificación
    Task<int> CountAsync(Expression<Func<Spare, bool>>? filter = null, CancellationToken ct = default);
    Task<bool> ExistsAsync(Expression<Func<Spare, bool>> filter, CancellationToken ct = default);
    Task<bool> CodigoExistsAsync(string Code, CancellationToken ct = default);

    // Operaciones de escritura
    Task AddAsync(Spare spare, CancellationToken ct = default);
    Task UpdateAsync(Spare spare, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    
    // Consultas específicas del negocio
    Task<IEnumerable<Spare>> GetRepuestosStockBajoAsync(int stockMin, CancellationToken ct = default);
    
    // Métodos adicionales útiles
    Task<PagedResult<Spare>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<Spare, bool>>? filter = null,
        Func<IQueryable<Spare>, IOrderedQueryable<Spare>>? orderBy = null,
        string includeProperties = "",
        CancellationToken ct = default);
        
    Task UpdateStockAsync(int id, int newStock, CancellationToken ct = default);
    Task<IEnumerable<Spare>> GetRepuestosPorCategoriaAsync(int categoryId, CancellationToken ct = default);
}