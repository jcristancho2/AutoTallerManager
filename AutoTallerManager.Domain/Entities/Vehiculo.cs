using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class Vehiculo : BaseEntity
    {
        public string? Placa { get; set; }
        public int Anio { get; set; }             
        public string? VIN { get; set; }
        public int Kilometraje { get; set; }

        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        public int TipoVehiculoId { get; set; }
        public TipoVehiculo? TipoVehiculo { get; set; }

        public int MarcaVehiculoId { get; set; }
        public MarcaVehiculo? MarcaVehiculo { get; set; }

        public int ModeloVehiculoId { get; set; }
        public ModeloVehiculo? ModeloVehiculo { get; set; }

        // Relación inversa típica (si usas OrdenServicio)
        public ICollection<OrdenServicio>? OrdenesServicio { get; set; }
    }
}