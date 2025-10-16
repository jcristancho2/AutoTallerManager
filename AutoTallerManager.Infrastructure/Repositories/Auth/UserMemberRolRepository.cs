using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Application.Abstractions.Auth;
using AutoTallerManager.Domain.Entities.Auth;
using AutoTallerManager.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AutoTallerManager.Infrastructure.Repositories.Auth;

public class UserMemberRolRepository(AppDbContext db) : IUserMemberRolService
{
    public async Task<IEnumerable<UserMemberRole>> GetAllAsync()
    {
        return await db.UserMemberRols.AsNoTracking().ToListAsync();
    }

    public async Task<UserMemberRole?> GetByIdsAsync(int userMemberId, int roleId)
    {
        return await db.UserMemberRols
            .FirstOrDefaultAsync(umr => umr.UserMemberId == userMemberId && umr.RoleId == roleId);
    }

    public void Remove(UserMemberRole entity)
    {
        db.UserMemberRols.Remove(entity);
    }

    public void Update(UserMemberRole entity)
    {
        db.UserMemberRols.Update(entity);
    }
}