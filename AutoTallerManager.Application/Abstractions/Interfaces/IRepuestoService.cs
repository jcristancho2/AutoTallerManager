using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using System.Linq.Expressions;

namespace AutoTallerManager.Application.Abstractions.Interfaces
{
    public interface IRepuestoService
    {
        Task<Repuesto?> GetByIdAsync(int id, CancellationToken ct = default, params string[] includeProperties);
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
        void Update(Repuesto repuesto);
        void Delete(Repuesto repuesto);
    }
}
