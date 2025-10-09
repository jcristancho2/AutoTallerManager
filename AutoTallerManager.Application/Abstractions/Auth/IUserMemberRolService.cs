using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Application.Abstractions.Auth
{
    public interface IUserMemberRolService
    {
        Task<UserMemberRol> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<UserMemberRol>> GetAllAsync(CancellationToken ct = default);
        Task AddAsync(UserMemberRol userMemberRol, CancellationToken ct = default);
        Task UpdateAsync(UserMemberRol userMemberRol, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}