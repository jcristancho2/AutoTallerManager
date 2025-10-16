using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities.Auth;

namespace AutoTallerManager.Application.Abstractions.Auth
{
    public interface IUserMemberRolService
    {
        Task<IEnumerable<UserMemberRole>> GetAllAsync();
        void Remove(UserMemberRole entity);
        void Update(UserMemberRole entity);
        Task<UserMemberRole?> GetByIdsAsync(int userMemberId, int roleId);
    }
}