using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities.Auth;
using System.Linq.Expressions;

namespace AutoTallerManager.Application.Abstractions.Auth
{
    public interface IRolService
    {
        Task<Rol?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<Rol>> GetAllAsync(CancellationToken ct = default);
        IEnumerable<Rol> Find(Expression<Func<Rol, bool>> expression);
        Task<IEnumerable<Rol>> GetPagedAsync(int pageIndex, int pageSize, string search, CancellationToken ct = default);
        Task<int> CountAsync(string? q, CancellationToken ct = default);
        Task AddAsync(Rol entity, CancellationToken ct = default);
        Task UpdateAsync(Rol entity, CancellationToken ct = default);
        Task RemoveAsync(Rol entity, CancellationToken ct = default);
    }
}