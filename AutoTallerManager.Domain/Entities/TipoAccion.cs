using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class TipoAccion
    {
        public int Id { get; private set; }
        public string? NombreAccion { get; private set; }
        
        private TipoAccion() { }

        public TipoAccion(string nombreAccion)
        {
            NombreAccion = nombreAccion;
        }
    }
}