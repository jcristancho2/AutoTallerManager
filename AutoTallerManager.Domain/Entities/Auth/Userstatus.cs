using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities.Auth
{
    public class UserStatus : BaseEntity
    {
        public int UserStatusId { get; set; } // Primary Key
        public string StatusName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}