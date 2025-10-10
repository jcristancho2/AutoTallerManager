using System.ComponentModel.DataAnnotations;
using AutoTallerManager.Domain.Entities.Auth;

namespace AutoTallerManager.Domain.Entities;

public class Usuario : BaseEntity
{
    
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public int RolId { get; set; }
    public int EstadoUsuarioId { get; set; }
    
    // Propiedades de navegación
    public virtual Rol Rol { get; set; } = null!;
    public virtual EstadoUsuario EstadoUsuario { get; set; } = null!;
    
    // Relaciones inversas
    public virtual ICollection<OrdenServicio> OrdenesServicio { get; set; } = new List<OrdenServicio>();
    public virtual ICollection<Auditoria> Auditorias { get; set; } = new List<Auditoria>();
}