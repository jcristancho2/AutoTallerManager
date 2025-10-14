using System;

namespace AutoTallerManager.Domain.Entities.Auth;

public class RefreshToken : BaseEntity
{
    public int UserId { get; set; }
    public virtual UserMember UserMember { get; set; } = null!;

    public string? Token { get; set; }
    public DateTime Expiries { get; set; }
    public bool Expired => DateTime.UtcNow >= Expiries;
    public DateTime CreatedDate { get; set; }
    public DateTime? Revoked { get; set; }
    public bool IsActive => Revoked == null && !Expired;
    public bool IsRevoked => Revoked != null;

}
