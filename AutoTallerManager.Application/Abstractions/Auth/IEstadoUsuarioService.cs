using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities.Auth;

namespace AutoTallerManager.Application.Abstractions.Auth
{
    public interface IEstadoUsuarioService
    {
        Task<EstadoUsuario?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<EstadoUsuario?> GetByNameAsync(string nombre, CancellationToken ct = default);
        Task<IEnumerable<EstadoUsuario>> GetAllAsync(CancellationToken ct = default);
        Task<EstadoUsuario> CreateAsync(EstadoUsuario estado, CancellationToken ct = default);
    }
}