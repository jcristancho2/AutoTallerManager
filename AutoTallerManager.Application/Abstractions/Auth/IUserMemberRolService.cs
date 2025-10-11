using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities.Auth;

namespace AutoTallerManager.Application.Abstractions.Auth
{
    public interface IUserMemberRolService
    {
        Task<IEnumerable<UserMemberRol>> GetAllAsync();
        void Remove(UserMemberRol entity);
        void Update(UserMemberRol entity);
        Task<UserMemberRol?> GetByIdsAsync(int userMemberId, int roleId);
    }
}