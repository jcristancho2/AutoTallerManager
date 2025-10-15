using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Abstractions.Interfaces;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AutoTallerManager.Infrastructure.Repositories;

public class VehicleRepository : IVehicleService
{
    private readonly AppDbContext _context;

    public VehicleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Vehicle?> GetByIdAsync(int id, CancellationToken ct = default, params string[] includeProperties)
    {
        IQueryable<Vehicle> query = _context.Vehicles;

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return await query.FirstOrDefaultAsync(v => v.Id == id, ct);
    }

    public async Task<Vehicle?> GetByVinAsync(string vin, CancellationToken ct = default)
    {
        return await _context.Vehicles
            .Include(v => v.Customer)
            .FirstOrDefaultAsync(v => v.VIN == vin, ct);
    }

    public async Task<IEnumerable<Vehicle>> GetAllAsync(
        Expression<Func<Vehicle, bool>>? filter = null,
        Func<IQueryable<Vehicle>, IOrderedQueryable<Vehicle>>? orderBy = null,
        string includeProperties = "",
        int? skip = null,
        int? take = null,
        CancellationToken ct = default)
    {
        IQueryable<Vehicle> query = _context.Vehicles;

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

    public async Task<int> CountAsync(Expression<Func<Vehicle, bool>>? filter = null, CancellationToken ct = default)
    {
        IQueryable<Vehicle> query = _context.Vehicles;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.CountAsync(ct);
    }

    public async Task<bool> ExistsAsync(Expression<Func<Vehicle, bool>> filter, CancellationToken ct = default)
    {
        return await _context.Vehicles.AnyAsync(filter, ct);
    }

    public async Task AddAsync(Vehicle vehicle, CancellationToken ct = default)
    {
        await _context.Vehicles.AddAsync(vehicle, ct);
    }

    public void Update(Vehicle vehicle)
    {
        _context.Vehicles.Update(vehicle);
    }

    public void Delete(Vehicle vehicle)
    {
        _context.Vehicles.Remove(vehicle);
    }
}