using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoTallerManager.Application.Abstractions.Auth;
using AutoTallerManager.Domain.Entities.Auth;
using AutoTallerManager.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AutoTallerManager.Infrastructure.Repositories.Auth;

public class RoleRepository(AppDbContext db) : IRoleService
{
    public async Task AddAsync(Role entity, CancellationToken ct = default)
    {
        db.Roles.Add(entity);
        // await db.SaveChangesAsync(ct);
        await Task.CompletedTask;
    }

    public Task<int> CountAsync(string? search, CancellationToken ct = default)
    {
        var query = db.Roles.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = $"%{search.Trim()}%";
            query = query.Where(p => EF.Functions.Like((p.RoleName ?? string.Empty).ToUpper(), $"%{term}%"));
        }
        return query.CountAsync(ct);
    }

    public IEnumerable<Role> Find(Expression<Func<Role, bool>> expression)
    {
        // Global tracking is disabled (UseQueryTrackingBehavior(NoTracking)).
        // For relationship changes we need tracked entities to avoid EF trying
        // to INSERT existing roles (causing duplicate PK errors). Use AsTracking here.
        return db.Set<Role>().AsTracking().Where(expression);
    }

    public async Task<IEnumerable<Role>> GetAllAsync(CancellationToken ct = default)
    {
        return await db.Roles.AsNoTracking().ToListAsync(ct);
    }

    public Task<Role?> GetByIdAsync(int id, CancellationToken ct = default)
        => db.Roles.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<IEnumerable<Role>> GetPagedAsync(int page, int size, string? q, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task RemoveAsync(Role entity, CancellationToken ct = default)
    {
        db.Roles.Remove(entity);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Role entity, CancellationToken ct = default)
    {
        db.Roles.Update(entity);
        await Task.CompletedTask;
    }

    IReadOnlyList<Role> IRoleService.Find(Expression<Func<Role, bool>> expression)
    {
        throw new NotImplementedException();
    }

    Task<IReadOnlyList<Role>> IRoleService.GetAllAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    Task<IReadOnlyList<Role>> IRoleService.GetPagedAsync(int page, int size, string? search, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}