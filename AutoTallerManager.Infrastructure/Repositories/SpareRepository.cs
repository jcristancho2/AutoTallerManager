using AutoTallerManager.Application.Abstractions.Interfaces;
using AutoTallerManager.Application.Common.Models;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AutoTallerManager.Infrastructure.Repositories;

public class SpareRepository : ISpareService
{
    private readonly AppDbContext _context;

    public SpareRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Spare?> GetByIdAsync(int id, CancellationToken ct = default, params string[] includeProperties)
    {
        IQueryable<Spare> query = _context.Spares;

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return await query.FirstOrDefaultAsync(r => r.Id == id, ct);
    }

    public async Task<Spare?> GetByCodigoAsync(string code, CancellationToken ct = default)
    {
        return await _context.Spares
            .Include(r => r.Category)
            .Include(r => r.Manufacturer)
            .Include(r => r.VehicleType)
            .FirstOrDefaultAsync(r => r.Code == code, ct);
    }

    public async Task<Spare?> GetByCodigoWithIncludesAsync(string code, CancellationToken ct = default, params string[] includeProperties)
    {
        IQueryable<Spare> query = _context.Spares;
        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return await query.FirstOrDefaultAsync(r => r.Code == code, ct);
    }

    public async Task<IEnumerable<Spare>> GetAllAsync(
        Expression<Func<Spare, bool>>? filter = null,
        Func<IQueryable<Spare>, IOrderedQueryable<Spare>>? orderBy = null,
        string includeProperties = "",
        int? skip = null,
        int? take = null,
        CancellationToken ct = default)
    {
        IQueryable<Spare> query = _context.Spares;

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

    public async Task<int> CountAsync(Expression<Func<Spare, bool>>? filter = null, CancellationToken ct = default)
    {
        IQueryable<Spare> query = _context.Spares;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.CountAsync(ct);
    }

    public async Task<bool> ExistsAsync(Expression<Func<Spare, bool>> filter, CancellationToken ct = default)
    {
        return await _context.Spares.AnyAsync(filter, ct);
    }

    public async Task<bool> CodigoExistsAsync(string code, CancellationToken ct = default)
    {
        return await _context.Spares.AnyAsync(r => r.Code == code, ct);
    }

    public async Task AddAsync(Spare spare, CancellationToken ct = default)
    {
        await _context.Spares.AddAsync(spare, ct);
    }

    public Task UpdateAsync(Spare spare, CancellationToken ct = default)
    {
        _context.Spares.Update(spare);
        return Task.CompletedTask;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var spare = await _context.Spares.FirstOrDefaultAsync(r => r.Id == id, ct);
        if (spare == null)
            return false;

        _context.Spares.Remove(spare);
        return true;
    }

    public async Task<IEnumerable<Spare>> GetRepuestosStockBajoAsync(int stockMinimo, CancellationToken ct = default)
    {
        return await _context.Spares
            .Where(r => r.Stock <= stockMinimo)
            .Include(r => r.Category)
            .Include(r => r.Manufacturer)
            .OrderBy(r => r.Stock)
            .ToListAsync(ct);
    }

    public async Task<PagedResult<Spare>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<Spare, bool>>? filter = null,
        Func<IQueryable<Spare>, IOrderedQueryable<Spare>>? orderBy = null,
        string includeProperties = "",
        CancellationToken ct = default)
    {
        IQueryable<Spare> query = _context.Spares;

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

        return new PagedResult<Spare>(items, totalCount, pageNumber, pageSize);
    }

    public async Task UpdateStockAsync(int id, int nuevoStock, CancellationToken ct = default)
    {
        var spare = await _context.Spares.FirstOrDefaultAsync(r => r.Id == id, ct);
        if (spare == null)
        {
            return;
        }

        spare.Stock = nuevoStock;
        _context.Spares.Update(spare);
    }

    public async Task<IEnumerable<Spare>> GetRepuestosPorCategoriaAsync(int categoriaId, CancellationToken ct = default)
    {
        return await _context.Spares
            .Where(r => r.CategoryId == categoriaId)
            .Include(r => r.Category)
            .ToListAsync(ct);
    }
}