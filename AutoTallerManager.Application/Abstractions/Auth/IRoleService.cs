using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities.Auth;

namespace AutoTallerManager.Application.Abstractions.Auth
{
    public interface IRoleService
    {
        Task<Role?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken ct = default);

        IReadOnlyList<Role> Find(Expression<Func<Role, bool>> expression);

        Task<IReadOnlyList<Role>> GetPagedAsync(
            int page, 
            int size, 
            string? search = null, 
            CancellationToken ct = default);

        Task<int> CountAsync(string? search = null, CancellationToken ct = default);

        Task AddAsync(Role entity, CancellationToken ct = default);

        Task UpdateAsync(Role entity, CancellationToken ct = default);

        Task RemoveAsync(Role entity, CancellationToken ct = default);
    }
}
