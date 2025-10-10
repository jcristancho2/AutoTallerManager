using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Application.Abstractions.Interfaces
{
    public interface IVehiculoService
    {
        Task<Vehiculo?> GetByIdAsync(int id, CancellationToken ct = default, params string[] includeProperties);

        Task<IEnumerable<Vehiculo>> GetAllAsync(
            Expression<Func<Vehiculo, bool>>? filter = null,
            Func<IQueryable<Vehiculo>, IOrderedQueryable<Vehiculo>>? orderBy = null,
            string includeProperties = "",
            int? skip = null,
            int? take = null,
            CancellationToken ct = default);

        Task<int> CountAsync(Expression<Func<Vehiculo, bool>>? filter = null, CancellationToken ct = default);

        Task<bool> ExistsAsync(Expression<Func<Vehiculo, bool>> filter, CancellationToken ct = default);

        Task AddAsync(Vehiculo vehiculo, CancellationToken ct = default);

        void Update(Vehiculo vehiculo);

        void Delete(Vehiculo vehiculo);

        // ✅ Nuevo método para buscar por VIN
        Task<Vehiculo?> GetByVinAsync(string vin, CancellationToken ct = default);
    }
}
