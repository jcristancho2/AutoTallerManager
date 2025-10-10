using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities.Auth;

public class Rol : BaseEntity
{
    public string? NombreRol { get; set; }
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;

    // Relación N:M con UserMember vía tabla intermedia
    public virtual ICollection<UserMemberRol> UserMemberRoles { get; set; } = new HashSet<UserMemberRol>();

    // Relación opcional con Usuario (si tienes otro sistema)
    //public virtual ICollection<Usuario> Usuarios { get; set; } = new HashSet<Usuario>();
}
