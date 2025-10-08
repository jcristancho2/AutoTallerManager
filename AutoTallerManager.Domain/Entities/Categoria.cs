using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class Categoria
    {
        public int CategoriaId { get; set; }
        public string? NombreCat { get; set; } 

               // Relación 1 - N con Repuesto
        public ICollection<Repuesto>? Repuestos { get; set; }

    }
}