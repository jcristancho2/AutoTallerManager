using AutoTallerManager.Application.Abstractions.Interfaces;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AutoTallerManager.Infrastructure.Repositories;

public class DetalleOrdenRepository : IDetailOrderService
{
    private readonly AppDbContext _context;

    public DetalleOrdenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OrderDetail?> GetByIdAsync(int detalleOrdenId, int ordenServicioId, CancellationToken ct = default)
    {
        return await _context.OrderDetails
            .Include(d => d.Spare)
            .Include(d => d.ServiceOrder)
            .FirstOrDefaultAsync(d => d.DetailOrderId == detalleOrdenId && d.ServiceOrderId == ordenServicioId, ct);
    }

    public async Task<IEnumerable<OrderDetail>> GetAllAsync(
        Expression<Func<OrderDetail, bool>>? filter = null,
        Func<IQueryable<OrderDetail>, IOrderedQueryable<OrderDetail>>? orderBy = null,
        string includeProperties = "",
        int? skip = null,
        int? take = null,
        CancellationToken ct = default)
    {
        IQueryable<OrderDetail> query = _context.OrderDetails;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProperty);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        if (skip.HasValue)
        {
            query = query.Skip(skip.Value);
        }

        if (take.HasValue)
        {
            query = query.Take(take.Value);
        }

        return await query.ToListAsync(ct);
    }

    public async Task<int> CountAsync(Expression<Func<OrderDetail, bool>>? filter = null, CancellationToken ct = default)
    {
        IQueryable<OrderDetail> query = _context.OrderDetails;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.CountAsync(ct);
    }

    public async Task<bool> ExistsAsync(Expression<Func<OrderDetail, bool>> filter, CancellationToken ct = default)
    {
        return await _context.OrderDetails.AnyAsync(filter, ct);
    }

    public async Task AddAsync(OrderDetail orderDetail, CancellationToken ct = default)
    {
        await _context.OrderDetails.AddAsync(orderDetail, ct);
    }

    public Task UpdateAsync(OrderDetail orderDetail, CancellationToken ct = default)
    {
        _context.OrderDetails.Update(orderDetail);
        return Task.CompletedTask;
    }

    public async Task<bool> DeleteAsync(int orderDetailId, int serviceOrderId, CancellationToken ct = default)
    {
        var orderDetail = await _context.OrderDetails
            .FirstOrDefaultAsync(d => d.DetailOrderId == orderDetailId && d.ServiceOrderId == serviceOrderId, ct);

        if (orderDetail == null)
            return false;

        _context.OrderDetails.Remove(orderDetail);
        return true;
    }

    public async Task<IEnumerable<OrderDetail>> GetDetallesByOrdenAsync(int ordenServicioId, CancellationToken ct = default)
    {
        return await _context.OrderDetails
            .Where(d => d.ServiceOrderId == ordenServicioId)
            .Include(d => d.Spare)
            .ToListAsync(ct);
    }
}
