using AutoTallerManager.Application.Abstractions.Interfaces;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AutoTallerManager.Infrastructure.Repositories;

public class AuditoriaRepository : IAuditoriaService
{
    private readonly AppDbContext _context;

    public AuditoriaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Auditoria?> GetByIdAsync(int id, CancellationToken ct = default, params string[] includeProperties)
    {
        IQueryable<Auditoria> query = _context.Auditorias;

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return await query.FirstOrDefaultAsync(a => a.Id == id, ct);
    }

    public async Task<IEnumerable<Auditoria>> GetAllAsync(
        Expression<Func<Auditoria, bool>>? filter = null,
        Func<IQueryable<Auditoria>, IOrderedQueryable<Auditoria>>? orderBy = null,
        string includeProperties = "",
        int? skip = null,
        int? take = null,
        CancellationToken ct = default)
    {
        IQueryable<Auditoria> query = _context.Auditorias;

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

    public async Task<int> CountAsync(Expression<Func<Auditoria, bool>>? filter = null, CancellationToken ct = default)
    {
        IQueryable<Auditoria> query = _context.Auditorias;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.CountAsync(ct);
    }

    public async Task AddAsync(Auditoria auditoria, CancellationToken ct = default)
    {
        await _context.Auditorias.AddAsync(auditoria, ct);
    }
}