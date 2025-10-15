using AutoTallerManager.Domain.Entities;
using System.Linq.Expressions;

namespace AutoTallerManager.Application.Abstractions.Interfaces;

public interface IDetailOrderService
{
    Task<OrderDetail?> GetByIdAsync(int detailOrderId, int serviceOrderId, CancellationToken ct = default);
    Task<IEnumerable<OrderDetail>> GetAllAsync(
        Expression<Func<OrderDetail, bool>>? filter = null,
        Func<IQueryable<OrderDetail>, IOrderedQueryable<OrderDetail>>? orderBy = null,
        string includeProperties = "",
        int? skip = null,
        int? take = null,
        CancellationToken ct = default);
    Task<int> CountAsync(Expression<Func<OrderDetail, bool>>? filter = null, CancellationToken ct = default);
    Task<bool> ExistsAsync(Expression<Func<OrderDetail, bool>> filter, CancellationToken ct = default);
    Task AddAsync(OrderDetail orderDetail, CancellationToken ct = default);
    Task UpdateAsync(OrderDetail orderDetail, CancellationToken ct = default);
    Task<bool> DeleteAsync(int orderDetailId, int serviceOrderId, CancellationToken ct = default);
    Task<IEnumerable<OrderDetail>> GetDetallesByOrdenAsync(int serviceOrderId, CancellationToken ct = default);
}


