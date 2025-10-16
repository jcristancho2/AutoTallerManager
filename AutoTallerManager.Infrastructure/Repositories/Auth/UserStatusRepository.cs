using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Application.Abstractions.Auth;
using AutoTallerManager.Domain.Entities.Auth;
using AutoTallerManager.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AutoTallerManager.Infrastructure.Repositories.Auth;

public class UserStatusRepository : IUserStatusService
{
    private readonly AppDbContext _context;

    public UserStatusRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserStatus?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.UserStatuses.FindAsync(new object[] { id }, ct);
    }

    public async Task<UserStatus?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        return await _context.UserStatuses
            .FirstOrDefaultAsync(e => e.StatusName == name, ct);
    }

    public async Task<IEnumerable<UserStatus>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.UserStatuses.ToListAsync(ct);
    }

    public async Task<UserStatus> CreateAsync(UserStatus status, CancellationToken ct = default)
    {
        await _context.UserStatuses.AddAsync(status, ct);
        await _context.SaveChangesAsync(ct);
        return status;
    }
}