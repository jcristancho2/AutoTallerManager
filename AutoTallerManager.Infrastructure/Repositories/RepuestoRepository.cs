using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Abstractions.Interfaces;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AutoTallerManager.Infrastructure.Repositories;

public class RepuestoRepository : IRepuestoService
{
    private readonly AppDbContext _context;

    public RepuestoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Repuesto?> GetByIdAsync(int id, CancellationToken ct = default, params string[] includeProperties)
    {
        IQueryable<Repuesto> query = _context.Repuestos;

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return await query.FirstOrDefaultAsync(r => r.RepuestoId == id, ct);
    }

    public async Task<Repuesto?> GetByCodigoAsync(string codigo, CancellationToken ct = default)
    {
        return await _context.Repuestos
            .FirstOrDefaultAsync(r => r.Codigo == codigo, ct);
    }

    public async Task<IEnumerable<Repuesto>> GetAllAsync(
        Expression<Func<Repuesto, bool>>? filter = null,
        Func<IQueryable<Repuesto>, IOrderedQueryable<Repuesto>>? orderBy = null,
        string includeProperties = "",
        int? skip = null,
        int? take = null,
        CancellationToken ct = default)
    {
        IQueryable<Repuesto> query = _context.Repuestos;

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

    public async Task<int> CountAsync(Expression<Func<Repuesto, bool>>? filter = null, CancellationToken ct = default)
    {
        IQueryable<Repuesto> query = _context.Repuestos;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.CountAsync(ct);
    }

    public async Task<bool> ExistsAsync(Expression<Func<Repuesto, bool>> filter, CancellationToken ct = default)
    {
        return await _context.Repuestos.AnyAsync(filter, ct);
    }

    public async Task AddAsync(Repuesto repuesto, CancellationToken ct = default)
    {
        await _context.Repuestos.AddAsync(repuesto, ct);
    }

    public void Update(Repuesto repuesto)
    {
        _context.Repuestos.Update(repuesto);
    }

    public void Delete(Repuesto repuesto)
    {
        _context.Repuestos.Remove(repuesto);
    }
}