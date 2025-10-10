using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities.Auth;

namespace AutoTallerManager.Application.Abstractions.Auth
{
    public interface IRolService
    {
        Task<Rol?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<IReadOnlyList<Rol>> GetAllAsync(CancellationToken ct = default);

        IReadOnlyList<Rol> Find(Expression<Func<Rol, bool>> expression);

        Task<IReadOnlyList<Rol>> GetPagedAsync(
            int page, 
            int size, 
            string? search = null, 
            CancellationToken ct = default);

        Task<int> CountAsync(string? search = null, CancellationToken ct = default);

        Task AddAsync(Rol entity, CancellationToken ct = default);

        Task UpdateAsync(Rol entity, CancellationToken ct = default);

        Task RemoveAsync(Rol entity, CancellationToken ct = default);
    }
}
