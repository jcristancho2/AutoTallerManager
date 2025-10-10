using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class Categoria: BaseEntity
    {
        public string? NombreCat { get; set; } 

               // Relación 1 - N con Repuesto
        public ICollection<Repuesto>? Repuestos { get; set; }

    }
}