using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Application.Abstractions.Interfaces
{
    public interface IClienteService
    {
        Task<Cliente?> GetByIdAsync(int id, CancellationToken ct = default, params string[] includeProperties);
        Task<Cliente?> GetByEmailAsync(string email, CancellationToken ct = default);

        Task<IReadOnlyList<Cliente>> GetAllAsync(
            Expression<Func<Cliente, bool>>? filter = null,
            Func<IQueryable<Cliente>, IOrderedQueryable<Cliente>>? orderBy = null,
            string includeProperties = "",
            int? skip = null,
            int? take = null,
            CancellationToken ct = default);

        Task<IReadOnlyList<Cliente>> GetPagedAsync(
            int page, int pageSize, string? search = null, CancellationToken ct = default);

        Task<int> CountAsync(Expression<Func<Cliente, bool>>? filter = null, CancellationToken ct = default);
        Task<bool> ExistsAsync(Expression<Func<Cliente, bool>> filter, CancellationToken ct = default);
        Task<bool> ExistsByIdAsync(int id, CancellationToken ct = default);
        Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);

        Task AddAsync(Cliente cliente, CancellationToken ct = default);
        Task UpdateAsync(Cliente cliente, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}