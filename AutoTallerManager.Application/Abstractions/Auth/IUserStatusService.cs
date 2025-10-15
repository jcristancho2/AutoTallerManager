using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities.Auth;

namespace AutoTallerManager.Application.Abstractions.Auth
{
    public interface IUserStatusService
    {
        Task<UserStatus?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<UserStatus?> GetByNameAsync(string name, CancellationToken ct = default);
        Task<IEnumerable<UserStatus>> GetAllAsync(CancellationToken ct = default);
        Task<UserStatus> CreateAsync(UserStatus status, CancellationToken ct = default);
    }
}