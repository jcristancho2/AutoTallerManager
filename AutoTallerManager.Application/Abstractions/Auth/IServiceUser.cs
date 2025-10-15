using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Application.Abstractions.Auth
{
    public interface IServiceUser
    {
        Task<User?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<IEnumerable<User>> GetAllAsync(
            Expression<Func<User, bool>>? filter = null,
            Func<IQueryable<User>, IOrderedQueryable<User>>? orderBy = null,
            string includeProperties = "",
            CancellationToken ct = default);

        Task<bool> ValidateCredentialsAsync(string email, string password, CancellationToken ct = default);

        Task<User> CreateAsync(User user, CancellationToken ct = default);
        Task<User> UpdateAsync(User user, CancellationToken ct = default);

        Task<bool> ChangePasswordAsync(int userId, string newPassword, CancellationToken ct = default);
        Task<bool> ActivateUserAsync(int userId, CancellationToken ct = default);
        Task<bool> DeactivateUserAsync(int userId, CancellationToken ct = default);
    }
}


