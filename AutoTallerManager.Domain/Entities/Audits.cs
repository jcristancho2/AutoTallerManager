using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities.Auth;

namespace AutoTallerManager.Domain.Entities
{
    public class Audits : BaseEntity
    {
        public int UserId { get; set; }
        public string? AffectedEntity { get; set; }
        public int ActionId { get; set; }
        public DateTime ActionDate { get; set; } = DateTime.UtcNow;
        public string? ActionDescription { get; set; }

    // Navegación
    public virtual UserMember User { get; set; } = null!;
    public virtual ActionType ActionType { get; set; } = null!;
    }
}