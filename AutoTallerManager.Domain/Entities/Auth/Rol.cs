using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities.Auth;

public class Rol : BaseEntity
{
    public int RolId { get; set; }
    public string? RoleName { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    // Relación N:M con UserMember vía tabla intermedia
    public virtual ICollection<UserMemberRol> UserMemberRoles { get; set; } = new HashSet<UserMemberRol>();

    public virtual ICollection<User> Users { get; set; } = new HashSet<User>();
}
