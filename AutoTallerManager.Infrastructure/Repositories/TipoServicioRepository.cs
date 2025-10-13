using AutoTallerManager.Application.Abstractions.Interfaces;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AutoTallerManager.Infrastructure.Repositories;

public class TipoServicioRepository : ITipoServicioService
{
    private readonly AppDbContext _context;

    public TipoServicioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TipoServicio?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.TiposServicio.FirstOrDefaultAsync(ts => ts.Id == id, ct);
    }

    public async Task<IEnumerable<TipoServicio>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.TiposServicio.ToListAsync(ct);
    }
}
