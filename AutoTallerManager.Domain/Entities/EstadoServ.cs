using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class EstadoServ
    {
       public int EstadoId { get; set; }
        public string? NombreEstServ { get; set; }
        public ICollection<OrdenServicio>? OrdenesServicio { get; set; } 
    }
}