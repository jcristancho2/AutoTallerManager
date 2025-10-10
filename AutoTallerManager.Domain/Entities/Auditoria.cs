using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class Auditoria : BaseEntity
    {
        public int UsuarioId { get; set; }
        public string? EntidadAfectada { get; set; }
        public int AccionId { get; set; }
        public DateTime FechaHora { get; set; } = DateTime.UtcNow;
        public string? DescripcionAccion { get; set; }

    // Navegación
    public virtual Usuario Usuario { get; set; } = null!;
    public virtual TipoAccion TipoAccion { get; set; } = null!;
    }
}