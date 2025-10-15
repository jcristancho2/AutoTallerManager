using AutoTallerManager.Application.Abstractions.Interfaces;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AutoTallerManager.Infrastructure.Repositories;

public class ServiceTypeRepository : IServiceTypeService
{
    private readonly AppDbContext _context;

    public ServiceTypeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceType?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.ServiceTypes.FirstOrDefaultAsync(ts => ts.Id == id, ct);
    }

    public async Task<IEnumerable<ServiceType>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.ServiceTypes.ToListAsync(ct);
    }
}
