using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Application.Abstractions.Auth;

namespace AutoTallerManager.Application.Abstractions
{
    public interface IUnitOfWork
    {

        IUserMemberService UserMembers { get; }
        IUserMemberRolService UserMemberRoles { get; }
        IRolService Roles { get; }
        // Task<int> SaveAsync();
        Task<int> SaveChanges(CancellationToken ct = default);
        Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken ct = default);

    }
}