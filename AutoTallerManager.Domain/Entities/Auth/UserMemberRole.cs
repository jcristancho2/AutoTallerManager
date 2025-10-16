using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities.Auth;

public class UserMemberRole
{
    public int UserMemberId { get; set; }
    public int RoleId { get; set; }

    // Navegación
    public virtual UserMember UserMember { get; set; } = null!;
    public virtual Role Role { get; set; } = null!;

    //public object RoleName { get; set; }
}

