using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities;

    public class ModeloVehiculo : BaseEntity
    {
        public string Nombre { get; set; } = string.Empty;

        private ModeloVehiculo() { }

    public ModeloVehiculo(string nombre)
    {
        Nombre = nombre;

    }
        
    public ICollection<Vehiculo> Vehiculos { get; set; } = new HashSet<Vehiculo>();

    }

