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

public sealed class CustomerRepository(AppDbContext db) : IServiceCustomer
{
    public async Task<Customer?> GetByIdAsync(int id, CancellationToken ct = default, params string[] includeProperties)
    {
        IQueryable<Customer> query = db.Customers.AsNoTracking();

        foreach (var includeProperty in includeProperties)
            query = query.Include(includeProperty);

        return await query.FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await db.Customers
            .AsNoTracking()
            .Include(c => c.Address)
            .Include(c => c.CustomerType)
            .FirstOrDefaultAsync(c => c.Email == email, ct);
    }

    public async Task<IReadOnlyList<Customer>> GetAllAsync(
        Expression<Func<Customer, bool>>? filter = null,
        Func<IQueryable<Customer>, IOrderedQueryable<Customer>>? orderBy = null,
        string includeProperties = "",
        int? skip = null,
        int? take = null,
        CancellationToken ct = default)
    {
        IQueryable<Customer> query = db.Customers.AsNoTracking();

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

    public Task<int> CountAsync(Expression<Func<Customer, bool>>? filter = null, CancellationToken ct = default)
    {
        IQueryable<Customer> query = db.Customers.AsNoTracking();

        if (filter != null)
            query = query.Where(filter);

        return query.CountAsync(ct);
    }

    public Task<bool> ExistsAsync(Expression<Func<Customer, bool>> filter, CancellationToken ct = default)
        => db.Customers.AsNoTracking().AnyAsync(filter, ct);

    public Task<bool> ExistsByIdAsync(int id, CancellationToken ct = default)
        => db.Customers.AsNoTracking().AnyAsync(c => c.Id == id, ct);

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
        => db.Customers.AsNoTracking().AnyAsync(c => c.Email == email, ct);

    public async Task AddAsync(Customer customer, CancellationToken ct = default)
    {
        await db.Customers.AddAsync(customer, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Customer customer, CancellationToken ct = default)
    {
        db.Customers.Update(customer);
        await db.SaveChangesAsync(ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var customer = await db.Customers.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (customer == null)
            return false;

        bool hasVehicles = await db.Vehicles.AnyAsync(v => v.CustomerId == id, ct);
        if (hasVehicles)
            throw new InvalidOperationException("Cannot delete customer because they have associated vehicles.");

        db.Customers.Remove(customer);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<IReadOnlyList<Customer>> GetPagedAsync(
        int page, int pageSize, string? search = null, CancellationToken ct = default)
    {
        var query = db.Customers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string term = search.Trim().ToUpper();
            query = query.Where(c =>
                EF.Functions.Like((c.FullName ?? string.Empty).ToUpper(), $"%{term}%") ||
                EF.Functions.Like((c.Phone ?? string.Empty).ToUpper(), $"%{term}%") ||
                EF.Functions.Like((c.Email ?? string.Empty).ToUpper(), $"%{term}%"));
        }

        return await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }
}
