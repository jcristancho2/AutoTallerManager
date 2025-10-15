using AutoTallerManager.Application.Abstractions.Interfaces;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AutoTallerManager.Infrastructure.Repositories;

public class RepositoryAudit : IAuditService
{
    private readonly AppDbContext _context;

    public RepositoryAudit(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Audit?> GetByIdAsync(int id, CancellationToken ct = default, params string[] includeProperties)
    {
        IQueryable<Audit> query = _context.Audits;

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return await query.FirstOrDefaultAsync(a => a.Id == id, ct);
    }

    public async Task<IEnumerable<Audit>> GetAllAsync(
        Expression<Func<Audit, bool>>? filter = null,
        Func<IQueryable<Audit>, IOrderedQueryable<Audit>>? orderBy = null,
        string includeProperties = "",
        int? skip = null,
        int? take = null,
        CancellationToken ct = default)
    {
        IQueryable<Audit> query = _context.Audits;

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

    public async Task<int> CountAsync(Expression<Func<Audit, bool>>? filter = null, CancellationToken ct = default)
    {
        IQueryable<Audit> query = _context.Audits;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.CountAsync(ct);
    }

    public async Task AddAsync(Audit audit, CancellationToken ct = default)
    {
        await _context.Audits.AddAsync(audit, ct);
    }
}