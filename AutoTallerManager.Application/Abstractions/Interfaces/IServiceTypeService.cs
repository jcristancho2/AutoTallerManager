using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Application.Abstractions.Interfaces;

public interface IServiceTypeService
{
    Task<ServiceType?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<ServiceType>> GetAllAsync(CancellationToken ct = default);
   
}


