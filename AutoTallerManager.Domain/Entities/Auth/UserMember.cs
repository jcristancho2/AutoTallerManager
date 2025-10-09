using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities.Auth;

    public class UserMember : BaseEntity
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public ICollection<Rol> Rols { get; set; } = new HashSet<Rol>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();
        public ICollection<UserMemberRol> UserMemberRols { get; set; } = new HashSet<UserMemberRol>();

    }
