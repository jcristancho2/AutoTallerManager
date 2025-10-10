using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Abstractions.Interfaces;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AutoTallerManager.Infrastructure.Repositories;

public class ClienteRepository : IClienteService
{
    private readonly AppDbContext _context;

    public ClienteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Cliente?> GetByIdAsync(int id, CancellationToken ct = default, params string[] includeProperties)
    {
        IQueryable<Cliente> query = _context.Clientes;

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return await query.FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<Cliente?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _context.Clientes
            .FirstOrDefaultAsync(c => c.Email == email, ct);
    }

    public async Task<IEnumerable<Cliente>> GetAllAsync(
        Expression<Func<Cliente, bool>>? filter = null,
        Func<IQueryable<Cliente>, IOrderedQueryable<Cliente>>? orderBy = null,
        string includeProperties = "",
        int? skip = null,
        int? take = null,
        CancellationToken ct = default)
    {
        IQueryable<Cliente> query = _context.Clientes;

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

    public async Task<int> CountAsync(Expression<Func<Cliente, bool>>? filter = null, CancellationToken ct = default)
    {
        IQueryable<Cliente> query = _context.Clientes;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.CountAsync(ct);
    }

    public async Task<bool> ExistsAsync(Expression<Func<Cliente, bool>> filter, CancellationToken ct = default)
    {
        return await _context.Clientes.AnyAsync(filter, ct);
    }

    public async Task AddAsync(Cliente cliente, CancellationToken ct = default)
    {
        await _context.Clientes.AddAsync(cliente, ct);
    }

    public void Update(Cliente cliente)
    {
        _context.Clientes.Update(cliente);
    }

    public void Delete(Cliente cliente)
    {
        _context.Clientes.Remove(cliente);
    }
}