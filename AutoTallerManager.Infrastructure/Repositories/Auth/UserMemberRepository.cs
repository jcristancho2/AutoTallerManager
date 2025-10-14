using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities.Auth;
using AutoTallerManager.Application.Abstractions.Auth;
using AutoTallerManager.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AutoTallerManager.Infrastructure.Repositories.Auth;

public class UserMemberRepository : IUserMemberService
{
    private readonly AppDbContext _db;

    public UserMemberRepository(AppDbContext db)
    {
        _db = db;
    }

    // 🔹 Contar usuarios opcionalmente filtrando por search
    public Task<int> CountAsync(string? search = null, CancellationToken ct = default)
    {
        var query = _db.UsersMembers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = $"%{search.Trim()}%";
            query = query.Where(u => u.Username != null && EF.Functions.ILike(u.Username, term));
        }

        return query.CountAsync(ct);
    }

    // 🔹 Obtener por username o email con roles y tokens
    public async Task<UserMember?> GetByUserNameAsync(string userName, CancellationToken ct = default)
    {
        return await _db.UsersMembers
            .Include(u => u.UserMemberRoles)
                .ThenInclude(umr => umr.Rol)
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => 
                (u.Username != null && EF.Functions.ILike(u.Username, userName)) ||
                (u.Email != null && EF.Functions.ILike(u.Email, userName)), ct);
    }

    // 🔹 Obtener por Id
    public Task<UserMember?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return _db.UsersMembers
            .AsNoTracking()
            .Include(u => u.UserMemberRoles)
                .ThenInclude(umr => umr.Rol)
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    // 🔹 Obtener usuario por refresh token
    public async Task<UserMember> GetByRefreshTokenAsync(string refreshToken)
    {
        var user = await _db.UsersMembers
            .Include(u => u.UserMemberRoles)
                .ThenInclude(umr => umr.Rol)
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));

        if (user == null)
            throw new InvalidOperationException("UserMember not found for the given refresh token.");

        return user;
    }

    // 🔹 Agregar usuario
    public async Task AddAsync(UserMember userMember, CancellationToken ct = default)
    {
        await _db.UsersMembers.AddAsync(userMember, ct);
        // No guardamos cambios aquí, se hace en el UnitOfWork
    }

    // 🔹 Actualizar usuario
    public Task UpdateAsync(UserMember userMember, CancellationToken ct = default)
    {
        _db.UsersMembers.Update(userMember);
        // No guardamos cambios aquí, se hace en el UnitOfWork
        return Task.CompletedTask;
    }

    // 🔹 Eliminar usuario
    public Task RemoveAsync(UserMember userMember, CancellationToken ct = default)
    {
        _db.UsersMembers.Remove(userMember);
        // No guardamos cambios aquí, se hace en el UnitOfWork
        return Task.CompletedTask;
    }

    // 🔹 Obtener todos los usuarios
    public async Task<IEnumerable<UserMember>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.UsersMembers
            .AsNoTracking()
            .Include(u => u.UserMemberRoles)
                .ThenInclude(umr => umr.Rol)
            .Include(u => u.RefreshTokens)
            .ToListAsync(ct);
    }

    // 🔹 Buscar usuarios con expresión LINQ
    public IEnumerable<UserMember> Find(Expression<Func<UserMember, bool>> expression)
    {
        return _db.UsersMembers
            .Include(u => u.UserMemberRoles)
                .ThenInclude(umr => umr.Rol)
            .Include(u => u.RefreshTokens)
            .Where(expression);
    }

    // 🔹 Paginación
    public async Task<(int totalRegistros, IEnumerable<UserMember> registros)> GetPagedAsync(int pageIndex, int pageSize, string? search = null)
    {
        var query = _db.UsersMembers
            .Include(u => u.UserMemberRoles)
                .ThenInclude(umr => umr.Rol)
            .Include(u => u.RefreshTokens)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = $"%{search.Trim()}%";
            query = query.Where(u => u.Username != null && EF.Functions.ILike(u.Username, term));
        }

        var totalRegistros = await query.CountAsync();
        var registros = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (totalRegistros, registros);
    }
}
