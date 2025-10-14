using System.ComponentModel.DataAnnotations;
using AutoTallerManager.Domain.Entities.Auth;

namespace AutoTallerManager.Domain.Entities;

public class User : BaseEntity
{
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public int RolId { get; set; }
    public int StatusId { get; set; }
    
    // Propiedades de navegación
    public virtual Rol Rol { get; set; } = null!;
    public virtual UserStatus UserStatus { get; set; } = null!;

    // Relaciones inversas
    public virtual ICollection<ServiceOrder> ServiceOrders { get; set; } = new List<ServiceOrder>();
    public virtual ICollection<Audits> Audits { get; set; } = new List<Audits>();
}