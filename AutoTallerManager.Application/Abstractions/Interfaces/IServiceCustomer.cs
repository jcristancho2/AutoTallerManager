using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Application.Abstractions.Interfaces
{
    public interface IServiceCustomer
    {
        Task<Customer?> GetByIdAsync(int id, CancellationToken ct = default, params string[] includeProperties);
        Task<Customer?> GetByEmailAsync(string email, CancellationToken ct = default);

        Task<IReadOnlyList<Customer>> GetAllAsync(
            Expression<Func<Customer, bool>>? filter = null,
            Func<IQueryable<Customer>, IOrderedQueryable<Customer>>? orderBy = null,
            string includeProperties = "",
            int? skip = null,
            int? take = null,
            CancellationToken ct = default);

        Task<IReadOnlyList<Customer>> GetPagedAsync(
            int page, int pageSize, string? search = null, CancellationToken ct = default);

        Task<int> CountAsync(Expression<Func<Customer, bool>>? filter = null, CancellationToken ct = default);
        Task<bool> ExistsAsync(Expression<Func<Customer, bool>> filter, CancellationToken ct = default);
        Task<bool> ExistsByIdAsync(int id, CancellationToken ct = default);
        Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);

        Task AddAsync(Customer customer, CancellationToken ct = default);
        Task UpdateAsync(Customer customer, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}