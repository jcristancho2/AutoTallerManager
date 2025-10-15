using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Application.Abstractions.Interfaces
{
    public interface IVehicleService
    {
        Task<Vehicle?> GetByIdAsync(int id, CancellationToken ct = default, params string[] includeProperties);

        Task<IEnumerable<Vehicle>> GetAllAsync(
            Expression<Func<Vehicle, bool>>? filter = null,
            Func<IQueryable<Vehicle>, IOrderedQueryable<Vehicle>>? orderBy = null,
            string includeProperties = "",
            int? skip = null,
            int? take = null,
            CancellationToken ct = default);

        Task<int> CountAsync(Expression<Func<Vehicle, bool>>? filter = null, CancellationToken ct = default);

        Task<bool> ExistsAsync(Expression<Func<Vehicle, bool>> filter, CancellationToken ct = default);

        Task AddAsync(Vehicle vehicle, CancellationToken ct = default);

        void Update(Vehicle vehicle);

        void Delete(Vehicle vehicle);

        // ✅ Nuevo método para buscar por VIN
        Task<Vehicle?> GetByVinAsync(string vin, CancellationToken ct = default);
    }
}
