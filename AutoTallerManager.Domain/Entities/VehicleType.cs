using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class VehicleType : BaseEntity  
    {
        public string? Vehicle_Type_Name { get; set; }

        // Relación 1 - N con Repuesto
        public ICollection<Spare>? Spares { get; set; }
        
        public ICollection<Vehicle>? Vehicles { get; set; }
    }
}