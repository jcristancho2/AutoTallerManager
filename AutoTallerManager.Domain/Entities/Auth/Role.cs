using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities.Auth;

public class Role : BaseEntity
{
    public int RoleId { get; set; }
    public string? RoleName { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    // Relación N:M con UserMember vía tabla intermedia
    public virtual ICollection<UserMemberRole> UserMemberRols { get; set; } = new HashSet<UserMemberRole>();

    public virtual ICollection<User> Users { get; set; } = new HashSet<User>();
}
