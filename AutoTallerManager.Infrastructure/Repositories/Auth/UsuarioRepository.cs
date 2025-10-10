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

public class UsuarioRepository : IUsuarioService
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Usuarios
            .Include(u => u.Rol)
            .Include(u => u.EstadoUsuario)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _context.Usuarios
            .Include(u => u.Rol)
            .Include(u => u.EstadoUsuario)
            .FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    public async Task<IEnumerable<Usuario>> GetAllAsync(
        Expression<Func<Usuario, bool>>? filter = null,
        Func<IQueryable<Usuario>, IOrderedQueryable<Usuario>>? orderBy = null,
        string includeProperties = "",
        CancellationToken ct = default)
    {
        IQueryable<Usuario> query = _context.Usuarios;

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
        var usuario = await GetByEmailAsync(email, ct);
        if (usuario == null || usuario.EstadoUsuario.NombreEstUsu != "Activo")
        {
            return false;
        }

        // Aquí implementarías la verificación de password hash
        // return BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash);
        return usuario.PasswordHash == password; // Temporal - usar hash en producción
    }

    public async Task<Usuario> CreateAsync(Usuario usuario, CancellationToken ct = default)
    {
        // Hash de la contraseña antes de guardar
        // usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuario.PasswordHash);
        
        await _context.Usuarios.AddAsync(usuario, ct);
        await _context.SaveChangesAsync(ct);
        return usuario;
    }

    public async Task<Usuario> UpdateAsync(Usuario usuario, CancellationToken ct = default)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync(ct);
        return usuario;
    }

    public async Task<bool> ChangePasswordAsync(int usuarioId, string newPassword, CancellationToken ct = default)
    {
        var usuario = await GetByIdAsync(usuarioId, ct);
        if (usuario == null) return false;

        // usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        usuario.PasswordHash = newPassword; // Temporal
        
        await UpdateAsync(usuario, ct);
        return true;
    }

    public async Task<bool> ActivateUserAsync(int usuarioId, CancellationToken ct = default)
    {
        var usuario = await GetByIdAsync(usuarioId, ct);
        if (usuario == null) return false;

        var estadoActivo = await _context.EstadosUsuario
            .FirstOrDefaultAsync(e => e.NombreEstUsu == "Activo", ct);
        
        if (estadoActivo == null) return false;

        usuario.EstadoUsuarioId = estadoActivo.Id;
        await UpdateAsync(usuario, ct);
        return true;
    }

    public async Task<bool> DeactivateUserAsync(int usuarioId, CancellationToken ct = default)
    {
        var usuario = await GetByIdAsync(usuarioId, ct);
        if (usuario == null) return false;

        var estadoInactivo = await _context.EstadosUsuario
            .FirstOrDefaultAsync(e => e.NombreEstUsu == "NoActivo", ct);
        
        if (estadoInactivo == null) return false;

        usuario.EstadoUsuarioId = estadoInactivo.Id;
        await UpdateAsync(usuario, ct);
        return true;
    }
}