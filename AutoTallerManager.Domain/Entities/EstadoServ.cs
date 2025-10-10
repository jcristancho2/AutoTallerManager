using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class EstadoServ : BaseEntity
    {
        public string? NombreEstServ { get; set; }
        public ICollection<OrdenServicio>? OrdenesServicio { get; set; } 
    }
}