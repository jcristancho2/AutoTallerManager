using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class ModeloVehiculo
    {
        public int Id { get; private set; }
        public string Nombre { get; private set; } = string.Empty;

        private ModeloVehiculo() { }

        public ModeloVehiculo(string nombre)
        {
            Nombre = nombre;
        }
    }
}