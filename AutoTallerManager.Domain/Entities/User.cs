using System.ComponentModel.DataAnnotations;
using AutoTallerManager.Domain.Entities.Auth;

namespace AutoTallerManager.Domain.Entities;

public class User : BaseEntity
{
    public int UserId { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public int RoleId { get; set; }
    public int StatusId { get; set; }
    
    // Propiedades de navegación
    public virtual Role Role { get; set; } = null!;
    public virtual UserStatus UserStatus { get; set; } = null!;

    // Relaciones inversas
    public virtual ICollection<ServiceOrder> ServiceOrders { get; set; } = new List<ServiceOrder>();
    public virtual ICollection<Audit> Audits { get; set; } = new List<Audit>();
}