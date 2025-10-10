using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class TipoVehiculo : BaseEntity  
    {
        public string? NombreTipoVehiculo { get; set; }

        // Relación 1 - N con Repuesto
        public ICollection<Repuesto>? Repuestos { get; set; }
        
        public ICollection<Vehiculo>? Vehiculos { get; set; }
    }
}