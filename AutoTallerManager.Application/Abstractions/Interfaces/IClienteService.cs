using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using System.Linq.Expressions;

namespace AutoTallerManager.Application.Abstractions.Interfaces
{
    public interface IClienteService
    {
        Task<Cliente?> GetByIdAsync(Guid id, CancellationToken ct = default, params string[] includeProperties);
        Task<Cliente?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<IEnumerable<Cliente>> GetAllAsync(
            Expression<Func<Cliente, bool>>? filter = null,
            Func<IQueryable<Cliente>, IOrderedQueryable<Cliente>>? orderBy = null,
            string includeProperties = "",
            int? skip = null,
            int? take = null,
            CancellationToken ct = default);
        Task<int> CountAsync(Expression<Func<Cliente, bool>>? filter = null, CancellationToken ct = default);
        Task<bool> ExistsAsync(Expression<Func<Cliente, bool>> filter, CancellationToken ct = default);
        Task AddAsync(Cliente cliente, CancellationToken ct = default);
        void Update(Cliente cliente);
        void Delete(Cliente cliente);
    }
}