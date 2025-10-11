using AutoTallerManager.Application.Abstractions.Interfaces;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AutoTallerManager.Infrastructure.Repositories;

public class OrdenServicioRepository : IOrdenServicioService
{
    private readonly AppDbContext _context;

    public OrdenServicioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OrdenServicio?> GetByIdAsync(int id, CancellationToken ct = default, params string[] includeProperties)
    {
        IQueryable<OrdenServicio> query = _context.OrdenesServicio;

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return await query.FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task<IEnumerable<OrdenServicio>> GetAllAsync(
        Expression<Func<OrdenServicio, bool>>? filter = null,
        Func<IQueryable<OrdenServicio>, IOrderedQueryable<OrdenServicio>>? orderBy = null,
        string includeProperties = "",
        int? skip = null,
        int? take = null,
        CancellationToken ct = default)
    {
        IQueryable<OrdenServicio> query = _context.OrdenesServicio;

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

    public async Task<int> CountAsync(Expression<Func<OrdenServicio, bool>>? filter = null, CancellationToken ct = default)
    {
        IQueryable<OrdenServicio> query = _context.OrdenesServicio;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.CountAsync(ct);
    }

    public async Task<bool> ExistsAsync(Expression<Func<OrdenServicio, bool>> filter, CancellationToken ct = default)
    {
        return await _context.OrdenesServicio.AnyAsync(filter, ct);
    }

    public async Task AddAsync(OrdenServicio ordenServicio, CancellationToken ct = default)
    {
        await _context.OrdenesServicio.AddAsync(ordenServicio, ct);
    }

    public Task UpdateAsync(OrdenServicio ordenServicio, CancellationToken ct = default)
    {
        _context.OrdenesServicio.Update(ordenServicio);
        return Task.CompletedTask;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var ordenServicio = await _context.OrdenesServicio.FirstOrDefaultAsync(o => o.Id == id, ct);
        if (ordenServicio == null)
            return false;

        _context.OrdenesServicio.Remove(ordenServicio);
        return true;
    }
}