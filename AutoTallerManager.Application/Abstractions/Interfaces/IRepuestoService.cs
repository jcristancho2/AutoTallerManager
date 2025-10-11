using AutoTallerManager.Application.Common.Models;
using AutoTallerManager.Domain.Entities;
using System.Linq.Expressions;

namespace AutoTallerManager.Application.Abstractions.Interfaces;

public interface IRepuestoService
{
    // Operaciones de lectura
    Task<Repuesto?> GetByIdAsync(int id, CancellationToken ct = default, params string[] includeProperties);
    Task<Repuesto?> GetByCodigoAsync(string codigo, CancellationToken ct = default);
    Task<Repuesto?> GetByCodigoWithIncludesAsync(string codigo, CancellationToken ct = default, params string[] includeProperties);

    // Operaciones de consulta múltiple
    Task<IEnumerable<Repuesto>> GetAllAsync(
        Expression<Func<Repuesto, bool>>? filter = null,
        Func<IQueryable<Repuesto>, IOrderedQueryable<Repuesto>>? orderBy = null,
        string includeProperties = "",
        int? skip = null,
        int? take = null,
        CancellationToken ct = default);
    
    // Operaciones de conteo y verificación
    Task<int> CountAsync(Expression<Func<Repuesto, bool>>? filter = null, CancellationToken ct = default);
    Task<bool> ExistsAsync(Expression<Func<Repuesto, bool>> filter, CancellationToken ct = default);
    Task<bool> CodigoExistsAsync(string codigo, CancellationToken ct = default);

    // Operaciones de escritura
    Task AddAsync(Repuesto repuesto, CancellationToken ct = default);
    Task UpdateAsync(Repuesto repuesto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    
    // Consultas específicas del negocio
    Task<IEnumerable<Repuesto>> GetRepuestosStockBajoAsync(int stockMinimo, CancellationToken ct = default);
    
    // Métodos adicionales útiles
    Task<PagedResult<Repuesto>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<Repuesto, bool>>? filter = null,
        Func<IQueryable<Repuesto>, IOrderedQueryable<Repuesto>>? orderBy = null,
        string includeProperties = "",
        CancellationToken ct = default);
        
    Task UpdateStockAsync(int id, int nuevoStock, CancellationToken ct = default);
    Task<IEnumerable<Repuesto>> GetRepuestosPorCategoriaAsync(int categoriaId, CancellationToken ct = default);
}