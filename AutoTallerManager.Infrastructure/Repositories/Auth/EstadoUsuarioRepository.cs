using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Application.Abstractions.Auth;
using AutoTallerManager.Domain.Entities.Auth;
using AutoTallerManager.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AutoTallerManager.Infrastructure.Repositories.Auth;

public class EstadoUsuarioRepository : IEstadoUsuarioService
{
    private readonly AppDbContext _context;

    public EstadoUsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<EstadoUsuario?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.EstadosUsuario.FindAsync(new object[] { id }, ct);
    }

    public async Task<EstadoUsuario?> GetByNameAsync(string nombre, CancellationToken ct = default)
    {
        return await _context.EstadosUsuario
            .FirstOrDefaultAsync(e => e.NombreEstUsu == nombre, ct);
    }

    public async Task<IEnumerable<EstadoUsuario>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.EstadosUsuario.ToListAsync(ct);
    }

    public async Task<EstadoUsuario> CreateAsync(EstadoUsuario estado, CancellationToken ct = default)
    {
        await _context.EstadosUsuario.AddAsync(estado, ct);
        await _context.SaveChangesAsync(ct);
        return estado;
    }
}