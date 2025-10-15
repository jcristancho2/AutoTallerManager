using AutoTallerManager.Application.Abstractions.Interfaces;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AutoTallerManager.Infrastructure.Repositories;

public class RepositoryOrderService : IOrderServiceService
{
    private readonly AppDbContext _context;

    public RepositoryOrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceOrder?> GetByIdAsync(int id, CancellationToken ct = default, params string[] includeProperties)
    {
        IQueryable<ServiceOrder> query = _context.ServiceOrders;

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return await query.FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task<IEnumerable<ServiceOrder>> GetAllAsync(
        Expression<Func<ServiceOrder, bool>>? filter = null,
        Func<IQueryable<ServiceOrder>, IOrderedQueryable<ServiceOrder>>? orderBy = null,
        string includeProperties = "",
        int? skip = null,
        int? take = null,
        CancellationToken ct = default)
    {
        IQueryable<ServiceOrder> query = _context.ServiceOrders;

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

    public async Task<int> CountAsync(Expression<Func<ServiceOrder, bool>>? filter = null, CancellationToken ct = default)
    {
        IQueryable<ServiceOrder> query = _context.ServiceOrders;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.CountAsync(ct);
    }

    public async Task<bool> ExistsAsync(Expression<Func<ServiceOrder, bool>> filter, CancellationToken ct = default)
    {
        return await _context.ServiceOrders.AnyAsync(filter, ct);
    }

    public async Task AddAsync(ServiceOrder serviceOrder, CancellationToken ct = default)
    {
        await _context.ServiceOrders.AddAsync(serviceOrder, ct);
    }

    public Task UpdateAsync(ServiceOrder serviceOrder, CancellationToken ct = default)
    {
        _context.ServiceOrders.Update(serviceOrder);
        return Task.CompletedTask;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var serviceOrder = await _context.ServiceOrders.FirstOrDefaultAsync(o => o.Id == id, ct);
        if (serviceOrder == null)
            return false;

        _context.ServiceOrders.Remove(serviceOrder);
        return true;
    }
}