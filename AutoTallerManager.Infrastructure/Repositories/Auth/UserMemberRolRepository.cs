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
    public async Task<IEnumerable<UserMemberRol>> GetAllAsync()
    {
        return await db.UserMemberRols.AsNoTracking().ToListAsync();
    }

    public async Task<UserMemberRol?> GetByIdsAsync(int userMemberId, int roleId)
    {
        return await db.UserMemberRols
            .FirstOrDefaultAsync(umr => umr.UserMemberId == userMemberId && umr.RolId == roleId);
    }

    public void Remove(UserMemberRol entity)
    {
        db.UserMemberRols.Remove(entity);
    }

    public void Update(UserMemberRol entity)
    {
        db.UserMemberRols.Update(entity);
    }
}