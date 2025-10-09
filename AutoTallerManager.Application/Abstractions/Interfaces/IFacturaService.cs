using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using System.Linq.Expressions;

namespace AutoTallerManager.Application.Abstractions.Interfaces
{
    public interface IFacturaService
    {
        Task<Factura?> GetByIdAsync(int id, CancellationToken ct = default, params string[] includeProperties);
        Task<IEnumerable<Factura>> GetAllAsync(
            Expression<Func<Factura, bool>>? filter = null,
            Func<IQueryable<Factura>, IOrderedQueryable<Factura>>? orderBy = null,
            string includeProperties = "",
            int? skip = null,
            int? take = null,
            CancellationToken ct = default);
        Task<int> CountAsync(Expression<Func<Factura, bool>>? filter = null, CancellationToken ct = default);
        Task<bool> ExistsAsync(Expression<Func<Factura, bool>> filter, CancellationToken ct = default);
        Task AddAsync(Factura factura, CancellationToken ct = default);
        void Update(Factura factura);
        void Delete(Factura factura);
    }
}