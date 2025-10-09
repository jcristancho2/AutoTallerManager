using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using System.Linq.Expressions;

namespace AutoTallerManager.Application.Abstractions.Interfaces
{
    public interface IAuditoriaService
    {
        Task<Auditoria?> GetByIdAsync(int id, CancellationToken ct = default, params string[] includeProperties);
        Task<IEnumerable<Auditoria>> GetAllAsync(
            Expression<Func<Auditoria, bool>>? filter = null,
            Func<IQueryable<Auditoria>, IOrderedQueryable<Auditoria>>? orderBy = null,
            string includeProperties = "",
            int? skip = null,
            int? take = null,
            CancellationToken ct = default);
        Task<int> CountAsync(Expression<Func<Auditoria, bool>>? filter = null, CancellationToken ct = default);
        Task AddAsync(Auditoria auditoria, CancellationToken ct = default);
    }
}