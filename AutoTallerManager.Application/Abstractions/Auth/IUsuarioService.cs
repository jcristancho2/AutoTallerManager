using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Application.Abstractions.Auth
{
    public interface IUsuarioService
    {
        Task<Usuario?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<IEnumerable<Usuario>> GetAllAsync(
            Expression<Func<Usuario, bool>>? filter = null,
            Func<IQueryable<Usuario>, IOrderedQueryable<Usuario>>? orderBy = null,
            string includeProperties = "",
            CancellationToken ct = default);

        Task<bool> ValidateCredentialsAsync(string email, string password, CancellationToken ct = default);

        Task<Usuario> CreateAsync(Usuario usuario, CancellationToken ct = default);
        Task<Usuario> UpdateAsync(Usuario usuario, CancellationToken ct = default);

        Task<bool> ChangePasswordAsync(int usuarioId, string newPassword, CancellationToken ct = default);
        Task<bool> ActivateUserAsync(int usuarioId, CancellationToken ct = default);
        Task<bool> DeactivateUserAsync(int usuarioId, CancellationToken ct = default);
    }
}


