using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class Auditoria
    {
        public int Id { get; private set; }
        public int UsuarioId { get; private set; }
        public string? EntidadAfectada { get; private set; }
        public int AccionId { get; private set; }
        public DateTime FechaHora { get; private set; } = DateTime.UtcNow;
        public string? DescripcionAccion { get; private set; }

        private Auditoria() { }

        public Auditoria(int usuarioId, string entidadAfectada, int accionId, string descripcionAccion)
        {
            UsuarioId = usuarioId;
            EntidadAfectada = entidadAfectada;
            AccionId = accionId;
            DescripcionAccion = descripcionAccion;
        }
    }
}