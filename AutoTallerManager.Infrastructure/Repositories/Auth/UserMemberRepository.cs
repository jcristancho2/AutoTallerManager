using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities.Auth;
using AutoTallerManager.Application.Abstractions.Auth;
using AutoTallerManager.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AutoTallerManager.Infrastructure.Repositories.Auth;

public class UserMemberRepository (AppDbContext db) : IUserMemberService
{
    public Task<int> CountAsync(string? search = null, CancellationToken ct = default)
    {
        var query = db.UsersMembers.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = $"%{search.Trim()}%";
            query = query.Where(p => EF.Functions.ILike(p.Username, term));
        }
        return query.CountAsync(ct);
    }

    public async Task<UserMember?> GetByUserNameAsync(string userName, CancellationToken ct = default)
    {
        return await db.UsersMembers
                .Include(u => u.UserMemberRols)
                .Include(u => u.Rols)
                .FirstOrDefaultAsync(u => EF.Functions.ILike(u.Username, userName));

    }
    
     public virtual IEnumerable<UserMember> Find(Expression<Func<UserMember, bool>> expression)
    {
        return db.Set<UserMember>().Where(expression);
    }

    public Task<UserMember?> GetByIdAsync(int id, CancellationToken ct = default)
        => db.UsersMembers.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<UserMember> GetByRefreshTokenAsync(string refreshToken)
    {
        var user = await db.UsersMembers
                    .Include(u => u.Rols)
                    .Include(u => u.RefreshTokens)
                    .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));
        if (user == null)
            throw new InvalidOperationException("UserMember not found for the given refresh token.");
        return user;
    }


    public async Task AddAsync(UserMember usermember, CancellationToken ct = default)
    {
        db.UsersMembers.Add(usermember);
        // await db.SaveChangesAsync(ct);
        await Task.CompletedTask;
    }

    public async Task UpdateAsync(UserMember user_member, CancellationToken ct = default)
    {
        db.UsersMembers.Update(user_member);
        // await db.SaveChangesAsync(ct);
        await Task.CompletedTask;
    }

    public async Task RemoveAsync(UserMember user_member, CancellationToken ct = default)
    {
        db.UsersMembers.Remove(user_member);
        await db.SaveChangesAsync(ct);
    }

    async Task<IEnumerable<UserMember>> IUserMemberService.GetAllAsync(CancellationToken ct)
    {
        return await db.UsersMembers.AsNoTracking().ToListAsync(ct);
    }

    public async Task<(int totalRegistros, IEnumerable<UserMember> registros)> GetPagedAsync(int pageIndex, int pageSize, string search)
    {
        var totalRegistros = await db.Set<UserMember>()
                            .CountAsync();

        var registros = await db.Set<UserMember>()
                                .Skip((pageIndex - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

        return (totalRegistros, registros);
    }



}
