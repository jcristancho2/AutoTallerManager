using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Abstractions.Interfaces;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AutoTallerManager.Infrastructure.Repositories;

public class VehiculoRepository : IVehiculoService
{
    private readonly AppDbContext _context;

    public VehiculoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Vehiculo?> GetByIdAsync(int id, CancellationToken ct = default, params string[] includeProperties)
    {
        IQueryable<Vehiculo> query = _context.Vehiculos;

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return await query.FirstOrDefaultAsync(v => v.Id == id, ct);
    }

    public async Task<Vehiculo?> GetByVinAsync(string vin, CancellationToken ct = default)
    {
        return await _context.Vehiculos
            .Include(v => v.Cliente)
            .FirstOrDefaultAsync(v => v.VIN == vin, ct);
    }

    public async Task<IEnumerable<Vehiculo>> GetAllAsync(
        Expression<Func<Vehiculo, bool>>? filter = null,
        Func<IQueryable<Vehiculo>, IOrderedQueryable<Vehiculo>>? orderBy = null,
        string includeProperties = "",
        int? skip = null,
        int? take = null,
        CancellationToken ct = default)
    {
        IQueryable<Vehiculo> query = _context.Vehiculos;

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

    public async Task<int> CountAsync(Expression<Func<Vehiculo, bool>>? filter = null, CancellationToken ct = default)
    {
        IQueryable<Vehiculo> query = _context.Vehiculos;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.CountAsync(ct);
    }

    public async Task<bool> ExistsAsync(Expression<Func<Vehiculo, bool>> filter, CancellationToken ct = default)
    {
        return await _context.Vehiculos.AnyAsync(filter, ct);
    }

    public async Task AddAsync(Vehiculo vehiculo, CancellationToken ct = default)
    {
        await _context.Vehiculos.AddAsync(vehiculo, ct);
    }

    public void Update(Vehiculo vehiculo)
    {
        _context.Vehiculos.Update(vehiculo);
    }

    public void Delete(Vehiculo vehiculo)
    {
        _context.Vehiculos.Remove(vehiculo);
    }
}