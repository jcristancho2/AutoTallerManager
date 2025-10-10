using System;

namespace AutoTallerManager.Domain.Entities.Auth;

public class RefreshToken : BaseEntity
{
    public int UserId { get; set; }
    public virtual UserMember UserMember { get; set; } = null!;

    public string? Token { get; set; }
    public DateTime Expires { get; set; }
    public bool Expired => DateTime.UtcNow >= Expires;
    public DateTime Created { get; set; }
    public DateTime? Revoked { get; set; }
    public bool Active => Revoked == null && !Expired;
}
