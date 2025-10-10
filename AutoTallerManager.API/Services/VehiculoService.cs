using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Abstractions.Interfaces;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.API.Services
{
    public class VehiculoService : IVehiculoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VehiculoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Vehiculo?> GetByIdAsync(int id, CancellationToken ct = default, params string[] includeProperties)
        {
            return await _unitOfWork.Vehiculos.GetByIdAsync(id, ct, includeProperties);
        }

        public async Task<IEnumerable<Vehiculo>> GetAllAsync(
            Expression<Func<Vehiculo, bool>>? filter = null,
            Func<IQueryable<Vehiculo>, IOrderedQueryable<Vehiculo>>? orderBy = null,
            string includeProperties = "",
            int? skip = null,
            int? take = null,
            CancellationToken ct = default)
        {
            return await _unitOfWork.Vehiculos.GetAllAsync(filter, orderBy, includeProperties, skip, take, ct);
        }

        public async Task<int> CountAsync(Expression<Func<Vehiculo, bool>>? filter = null, CancellationToken ct = default)
        {
            return await _unitOfWork.Vehiculos.CountAsync(filter, ct);
        }

        public async Task<bool> ExistsAsync(Expression<Func<Vehiculo, bool>> filter, CancellationToken ct = default)
        {
            return await _unitOfWork.Vehiculos.ExistsAsync(filter, ct);
        }

        public async Task AddAsync(Vehiculo vehiculo, CancellationToken ct = default)
        {
            await _unitOfWork.Vehiculos.AddAsync(vehiculo, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }

        public void Update(Vehiculo vehiculo)
        {
            _unitOfWork.Vehiculos.Update(vehiculo);
        }

        public void Delete(Vehiculo vehiculo)
        {
            _unitOfWork.Vehiculos.Delete(vehiculo);
        }

        // ✅ Implementación de GetByVinAsync
        public async Task<Vehiculo?> GetByVinAsync(string vin, CancellationToken ct = default)
        {
            return await _unitOfWork.Vehiculos.GetAllAsync(
                filter: v => v.VIN == vin,
                ct: ct
            ).ContinueWith(t => t.Result.FirstOrDefault(), ct);
        }
    }
}