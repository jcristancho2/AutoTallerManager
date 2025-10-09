using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Abstractions.Auth;
using AutoTallerManager.Infrastructure.Persistence.Context;
using AutoTallerManager.Infrastructure.Repositories;
using AutoTallerManager.Infrastructure.Repositories.Auth;

namespace AutoTallerManager.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        UserMembers = new UserMemberRepository(_context);
        UserMemberRoles = new UserMemberRolRepository(_context);
        Roles = new RolRepository(_context);
    }

    public IUserMemberService UserMembers { get; }
    public IUserMemberRolService UserMemberRoles { get; }
    public IRolService Roles { get; }

    public Task<int> SaveChanges(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);

    public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken ct = default)
    {
        await using var tx = await _context.Database.BeginTransactionAsync(ct);
        try
        {
            await operation(ct);
            await _context.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }
}
