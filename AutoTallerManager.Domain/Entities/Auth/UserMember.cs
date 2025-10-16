using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AutoTallerManager.Domain.Entities.Auth;

public class UserMember : BaseEntity
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }

    // N:M con Rol mediante tabla intermedia UserMemberRol
    public virtual ICollection<UserMemberRole> UserMemberRols { get; set; } = new HashSet<UserMemberRole>();

    public ICollection<Audit> Audits { get; set; } = new List<Audit>();
    
    // Relación con RefreshTokens
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();
}
