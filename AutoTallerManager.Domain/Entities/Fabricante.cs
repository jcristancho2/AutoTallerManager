using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class Fabricante
    {
        public int FabricanteId { get; set; }
        public string? NombreFab { get; set; }

        public string? Descripcion { get; set; }

        public string? Telefono { get; set; }

        public string? Correo { get; set; }

        public ICollection<Repuesto>? Repuestos { get; set; }
        
    }
}