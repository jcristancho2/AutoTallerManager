using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities.Auth
{
    public class EstadoUsuario : BaseEntity
    {

        public string NombreEstUsu { get; set; } = string.Empty;
        
        public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}