using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class TipoAccion : BaseEntity
    {
        public string? NombreAccion { get; set; }
        
        private TipoAccion() { }

        public TipoAccion(string nombreAccion)
        {
            NombreAccion = nombreAccion;
        }
    }
}