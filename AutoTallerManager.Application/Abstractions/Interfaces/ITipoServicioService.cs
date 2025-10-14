using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Application.Abstractions.Interfaces;

public interface ITipoServicioService
{
    Task<TipoServicio?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<TipoServicio>> GetAllAsync(CancellationToken ct = default);
}


