using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class MarcaVehiculo : BaseEntity
    {
        public string Nombre { get; set; } = string.Empty;

        public ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
    }
}