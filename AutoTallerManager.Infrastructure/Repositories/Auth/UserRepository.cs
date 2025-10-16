using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Application.Abstractions.Auth;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AutoTallerManager.Infrastructure.Repositories.Auth;

public class UserRepository : IUserService
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Users
            .Include(u => u.Role)
            .Include(u => u.UserStatus)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _context.Users
            .Include(u => u.Role)
            .Include(u => u.UserStatus)
            .FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    public async Task<IEnumerable<User>> GetAllAsync(
        Expression<Func<User, bool>>? filter = null,
        Func<IQueryable<User>, IOrderedQueryable<User>>? orderBy = null,
        string includeProperties = "",
        CancellationToken ct = default)
    {
        IQueryable<User> query = _context.Users;

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

        return await query.ToListAsync(ct);
    }

    public async Task<bool> ValidateCredentialsAsync(string email, string password, CancellationToken ct = default)
    {
        var user = await GetByEmailAsync(email, ct);
        if (user == null || user.UserStatus.StatusName != "Activo")
        {
            return false;
        }

        // Aquí implementarías la verificación de password hash
        // return BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash);
        return user.PasswordHash == password; // Temporal - usar hash en producción
    }

    public async Task<User> CreateAsync(User user, CancellationToken ct = default)
    {
        // Hash de la contraseña antes de guardar
        // usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuario.PasswordHash);
        
        await _context.Users.AddAsync(user, ct);
        await _context.SaveChangesAsync(ct);
        return user;
    }

    public async Task<User> UpdateAsync(User user, CancellationToken ct = default)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(ct);
        return user;
    }

    public async Task<bool> ChangePasswordAsync(int UserId, string newPassword, CancellationToken ct = default)
    {
        var user = await GetByIdAsync(UserId, ct);
        if (user == null) return false;

        // usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.PasswordHash = newPassword; // Temporal
        
        await UpdateAsync(user, ct);
        return true;
    }

    public async Task<bool> ActivateUserAsync(int UserId, CancellationToken ct = default)
    {
        var user = await GetByIdAsync(UserId, ct);
        if (user == null) return false;

        var activeStatus = await _context.UserStatuses
            .FirstOrDefaultAsync(e => e.StatusName == "Activo", ct);
        
        if (activeStatus == null) return false;

        user.UserStatus = activeStatus;
        await UpdateAsync(user, ct);
        return true;
    }

    public async Task<bool> DeactivateUserAsync(int UserId, CancellationToken ct = default)
    {
        var user = await GetByIdAsync(UserId, ct);
        if (user == null) return false;

        var inactiveStatus = await _context.UserStatuses
            .FirstOrDefaultAsync(e => e.StatusName == "NoActivo", ct);
        
        if (inactiveStatus == null) return false;

        user.UserStatus = inactiveStatus;
        await UpdateAsync(user, ct);
        return true;
    }
}