using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Infrastructure.Abstractions.Auth;
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


    
    









}
