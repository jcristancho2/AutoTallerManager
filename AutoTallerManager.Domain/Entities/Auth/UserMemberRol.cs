using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities.Auth;

public class UserMemberRol : BaseEntity
{
    public int UserMemberId { get; set; }
    public int RolId { get; set; }

    // Navegación
    public virtual UserMember UserMember { get; set; } = null!;
    public virtual Rol Rol { get; set; } = null!;
    public object NombreRol { get; set; }
}

