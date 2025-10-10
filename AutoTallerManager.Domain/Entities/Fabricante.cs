using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class Fabricante : BaseEntity
    {
        public string? NombreFab { get; set; }

        public string? Descripcion { get; set; }

        public string? Telefono { get; set; }

        public string? Email { get; set; }

        public ICollection<Repuesto>? Repuestos { get; set; }
        
    }
}