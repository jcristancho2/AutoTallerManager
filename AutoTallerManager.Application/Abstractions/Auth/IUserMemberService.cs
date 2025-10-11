using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities.Auth;
using System.Linq.Expressions;

namespace AutoTallerManager.Application.Abstractions.Auth
{
    public interface IUserMemberService
    {
        Task<UserMember?> GetByIdAsync(int id, CancellationToken ct = default);
        IEnumerable<UserMember> Find(Expression<Func<UserMember, bool>> expression);
        Task<IEnumerable<UserMember>> GetAllAsync(CancellationToken ct = default);
        Task<(int totalRegistros, IEnumerable<UserMember> registros)> GetPagedAsync(int pageIndex, int pageSize, string search);
        Task<int> CountAsync(string? q, CancellationToken ct = default);
        Task AddAsync(UserMember entity, CancellationToken ct = default);
        Task UpdateAsync(UserMember entity, CancellationToken ct = default);
        Task RemoveAsync(UserMember entity, CancellationToken ct = default);
        Task<UserMember?> GetByUserNameAsync(string userName, CancellationToken ct = default);
        Task<UserMember> GetByRefreshTokenAsync(string refreshToken);
    }
}