using AutoTallerManager.Application.Abstractions.Interfaces;
using AutoTallerManager.Application.Common.Models;
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

        return await query.FirstOrDefaultAsync(r => r.Id == id, ct);
    }

    public async Task<Repuesto?> GetByCodigoAsync(string codigo, CancellationToken ct = default)
    {
        return await _context.Repuestos
            .Include(r => r.Categoria)
            .Include(r => r.Fabricante)
            .Include(r => r.TipoVehiculo)
            .FirstOrDefaultAsync(r => r.Codigo == codigo, ct);
    }

    public async Task<Repuesto?> GetByCodigoWithIncludesAsync(string codigo, CancellationToken ct = default, params string[] includeProperties)
    {
        IQueryable<Repuesto> query = _context.Repuestos;
        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return await query.FirstOrDefaultAsync(r => r.Codigo == codigo, ct);
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

    public async Task<bool> CodigoExistsAsync(string codigo, CancellationToken ct = default)
    {
        return await _context.Repuestos.AnyAsync(r => r.Codigo == codigo, ct);
    }

    public async Task AddAsync(Repuesto repuesto, CancellationToken ct = default)
    {
        await _context.Repuestos.AddAsync(repuesto, ct);
    }

    public Task UpdateAsync(Repuesto repuesto, CancellationToken ct = default)
    {
        _context.Repuestos.Update(repuesto);
        return Task.CompletedTask;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var repuesto = await _context.Repuestos.FirstOrDefaultAsync(r => r.Id == id, ct);
        if (repuesto == null)
            return false;

        _context.Repuestos.Remove(repuesto);
        return true;
    }

    public async Task<IEnumerable<Repuesto>> GetRepuestosStockBajoAsync(int stockMinimo, CancellationToken ct = default)
    {
        return await _context.Repuestos
            .Where(r => r.Stock <= stockMinimo)
            .Include(r => r.Categoria)
            .Include(r => r.Fabricante)
            .OrderBy(r => r.Stock)
            .ToListAsync(ct);
    }

    public async Task<PagedResult<Repuesto>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<Repuesto, bool>>? filter = null,
        Func<IQueryable<Repuesto>, IOrderedQueryable<Repuesto>>? orderBy = null,
        string includeProperties = "",
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

        var totalCount = await query.CountAsync(ct);

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<Repuesto>(items, totalCount, pageNumber, pageSize);
    }

    public async Task UpdateStockAsync(int id, int nuevoStock, CancellationToken ct = default)
    {
        var repuesto = await _context.Repuestos.FirstOrDefaultAsync(r => r.Id == id, ct);
        if (repuesto == null)
        {
            return;
        }

        repuesto.Stock = nuevoStock;
        _context.Repuestos.Update(repuesto);
    }

    public async Task<IEnumerable<Repuesto>> GetRepuestosPorCategoriaAsync(int categoriaId, CancellationToken ct = default)
    {
        return await _context.Repuestos
            .Where(r => r.CategoriaId == categoriaId)
            .Include(r => r.Categoria)
            .ToListAsync(ct);
    }
}