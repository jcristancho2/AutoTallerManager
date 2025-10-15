using AutoTallerManager.Domain.Entities;
using System.Linq.Expressions;

namespace AutoTallerManager.Application.Abstractions.Interfaces;

public interface IInvoiceService
{
    Task<Invoice?> GetByIdAsync(int id, CancellationToken ct = default, params string[] includeProperties);
    Task<IEnumerable<Invoice>> GetAllAsync(
        Expression<Func<Invoice, bool>>? filter = null,
        Func<IQueryable<Invoice>, IOrderedQueryable<Invoice>>? orderBy = null,
        string includeProperties = "",
        int? skip = null,
        int? take = null,
        CancellationToken ct = default);
    Task<int> CountAsync(Expression<Func<Invoice, bool>>? filter = null, CancellationToken ct = default);
    Task<bool> ExistsAsync(Expression<Func<Invoice, bool>> filter, CancellationToken ct = default);
    Task AddAsync(Invoice invoice, CancellationToken ct = default);
    Task UpdateAsync(Invoice invoice, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Invoice>> GetFacturasByClienteAsync(int customerId, CancellationToken ct = default);
    Task<decimal> GetTotalIngresosAsync(DateTime dateFrom, DateTime dateTo, CancellationToken ct = default);
    Task<Invoice?> GetByServiceOrderIdAsync(int serviceOrderId, CancellationToken ct = default);
    Task<Invoice?> GetLastInvoiceAsync(CancellationToken ct = default);
}