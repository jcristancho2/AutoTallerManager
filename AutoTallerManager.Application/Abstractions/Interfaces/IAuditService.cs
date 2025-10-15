using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using System.Linq.Expressions;

namespace AutoTallerManager.Application.Abstractions.Interfaces
{
    public interface IAuditService
    {
        Task<Audit?> GetByIdAsync(int id, CancellationToken ct = default, params string[] includeProperties);
        Task<IEnumerable<Audit>> GetAllAsync(
            Expression<Func<Audit, bool>>? filter = null,
            Func<IQueryable<Audit>, IOrderedQueryable<Audit>>? orderBy = null,
            string includeProperties = "",
            int? skip = null,
            int? take = null,
            CancellationToken ct = default);
        Task<int> CountAsync(Expression<Func<Audit, bool>>? filter = null, CancellationToken ct = default);
        Task AddAsync(Audit audit, CancellationToken ct = default);
    }
}