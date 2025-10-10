using AutoTallerManager.Application.Abstractions.Interfaces;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace AutoTallerManager.Infrastructure.Repositories;

public sealed class ClienteRepository(AppDbContext db) : IClienteService
{
    public async Task<Cliente?> GetByIdAsync(int id, CancellationToken ct = default, params string[] includeProperties)
    {
        IQueryable<Cliente> query = db.Clientes.AsNoTracking();

        foreach (var includeProperty in includeProperties)
            query = query.Include(includeProperty);

        return await query.FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<Cliente?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await db.Clientes
            .AsNoTracking()
            .Include(c => c.Direccion)
            .Include(c => c.TipoCliente)
            .FirstOrDefaultAsync(c => c.Email == email, ct);
    }

    public async Task<IReadOnlyList<Cliente>> GetAllAsync(
        Expression<Func<Cliente, bool>>? filter = null,
        Func<IQueryable<Cliente>, IOrderedQueryable<Cliente>>? orderBy = null,
        string includeProperties = "",
        int? skip = null,
        int? take = null,
        CancellationToken ct = default)
    {
        IQueryable<Cliente> query = db.Clientes.AsNoTracking();

        if (filter != null)
            query = query.Where(filter);

        foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
            query = query.Include(includeProperty);

        if (orderBy != null)
            query = orderBy(query);

        if (skip.HasValue)
            query = query.Skip(skip.Value);

        if (take.HasValue)
            query = query.Take(take.Value);

        return await query.ToListAsync(ct);
    }

    public Task<int> CountAsync(Expression<Func<Cliente, bool>>? filter = null, CancellationToken ct = default)
    {
        IQueryable<Cliente> query = db.Clientes.AsNoTracking();

        if (filter != null)
            query = query.Where(filter);

        return query.CountAsync(ct);
    }

    public Task<bool> ExistsAsync(Expression<Func<Cliente, bool>> filter, CancellationToken ct = default)
        => db.Clientes.AsNoTracking().AnyAsync(filter, ct);

    public Task<bool> ExistsByIdAsync(int id, CancellationToken ct = default)
        => db.Clientes.AsNoTracking().AnyAsync(c => c.Id == id, ct);

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
        => db.Clientes.AsNoTracking().AnyAsync(c => c.Email == email, ct);

    public async Task AddAsync(Cliente cliente, CancellationToken ct = default)
    {
        await db.Clientes.AddAsync(cliente, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Cliente cliente, CancellationToken ct = default)
    {
        db.Clientes.Update(cliente);
        await db.SaveChangesAsync(ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var cliente = await db.Clientes.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (cliente == null)
            return false;

        bool tieneVehiculos = await db.Vehiculos.AnyAsync(v => v.ClienteId == id, ct);
        if (tieneVehiculos)
            throw new InvalidOperationException("No se puede eliminar el cliente porque tiene vehículos asociados.");

        db.Clientes.Remove(cliente);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<IReadOnlyList<Cliente>> GetPagedAsync(
        int page, int pageSize, string? search = null, CancellationToken ct = default)
    {
        var query = db.Clientes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string term = search.Trim().ToUpper();
            query = query.Where(c =>
                EF.Functions.Like((c.NombreCompleto ?? string.Empty).ToUpper(), $"%{term}%") ||
                EF.Functions.Like((c.Telefono ?? string.Empty).ToUpper(), $"%{term}%") ||
                EF.Functions.Like((c.Email ?? string.Empty).ToUpper(), $"%{term}%"));
        }

        return await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }
}
