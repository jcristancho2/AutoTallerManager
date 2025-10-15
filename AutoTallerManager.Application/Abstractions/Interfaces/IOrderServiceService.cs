using AutoTallerManager.Domain.Entities;
using System.Linq.Expressions;

namespace AutoTallerManager.Application.Abstractions.Interfaces;

public interface IOrderServiceService
{
    Task<ServiceOrder?> GetByIdAsync(int id, CancellationToken ct = default, params string[] includeProperties);
    Task<IEnumerable<ServiceOrder>> GetAllAsync(
        Expression<Func<ServiceOrder, bool>>? filter = null,
        Func<IQueryable<ServiceOrder>, IOrderedQueryable<ServiceOrder>>? orderBy = null,
        string includeProperties = "",
        int? skip = null,
        int? take = null,
        CancellationToken ct = default);
    Task<int> CountAsync(Expression<Func<ServiceOrder, bool>>? filter = null, CancellationToken ct = default);
    Task<bool> ExistsAsync(Expression<Func<ServiceOrder, bool>> filter, CancellationToken ct = default);
    Task AddAsync(ServiceOrder serviceOrder, CancellationToken ct = default);
    Task UpdateAsync(ServiceOrder serviceOrder, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}