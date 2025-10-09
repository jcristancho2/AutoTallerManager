using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities.Auth;

    public class Rol : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<UserMember> UsersMembers { get; set; } = new HashSet<UserMember>();
        public ICollection<UserMemberRol> UserMemberRols { get; set; } = new HashSet<UserMemberRol>();
    }
