using AutoTallerManager.Application.Abstractions.Interfaces;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AutoTallerManager.Infrastructure.Repositories;

public class DetalleOrdenRepository : IDetalleOrdenService
{
    private readonly AppDbContext _context;

    public DetalleOrdenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DetalleOrden?> GetByIdAsync(int detalleOrdenId, int ordenServicioId, CancellationToken ct = default)
    {
        return await _context.DetalleOrdenes
            .Include(d => d.Repuesto)
            .Include(d => d.OrdenServicio)
            .FirstOrDefaultAsync(d => d.DetalleOrdenId == detalleOrdenId && d.OrdenServicioId == ordenServicioId, ct);
    }

    public async Task<IEnumerable<DetalleOrden>> GetAllAsync(
        Expression<Func<DetalleOrden, bool>>? filter = null,
        Func<IQueryable<DetalleOrden>, IOrderedQueryable<DetalleOrden>>? orderBy = null,
        string includeProperties = "",
        int? skip = null,
        int? take = null,
        CancellationToken ct = default)
    {
        IQueryable<DetalleOrden> query = _context.DetalleOrdenes;

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

    public async Task<int> CountAsync(Expression<Func<DetalleOrden, bool>>? filter = null, CancellationToken ct = default)
    {
        IQueryable<DetalleOrden> query = _context.DetalleOrdenes;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.CountAsync(ct);
    }

    public async Task<bool> ExistsAsync(Expression<Func<DetalleOrden, bool>> filter, CancellationToken ct = default)
    {
        return await _context.DetalleOrdenes.AnyAsync(filter, ct);
    }

    public async Task AddAsync(DetalleOrden detalleOrden, CancellationToken ct = default)
    {
        await _context.DetalleOrdenes.AddAsync(detalleOrden, ct);
    }

    public Task UpdateAsync(DetalleOrden detalleOrden, CancellationToken ct = default)
    {
        _context.DetalleOrdenes.Update(detalleOrden);
        return Task.CompletedTask;
    }

    public async Task<bool> DeleteAsync(int detalleOrdenId, int ordenServicioId, CancellationToken ct = default)
    {
        var detalleOrden = await _context.DetalleOrdenes
            .FirstOrDefaultAsync(d => d.DetalleOrdenId == detalleOrdenId && d.OrdenServicioId == ordenServicioId, ct);
        
        if (detalleOrden == null)
            return false;

        _context.DetalleOrdenes.Remove(detalleOrden);
        return true;
    }

    public async Task<IEnumerable<DetalleOrden>> GetDetallesByOrdenAsync(int ordenServicioId, CancellationToken ct = default)
    {
        return await _context.DetalleOrdenes
            .Where(d => d.OrdenServicioId == ordenServicioId)
            .Include(d => d.Repuesto)
            .ToListAsync(ct);
    }
}
