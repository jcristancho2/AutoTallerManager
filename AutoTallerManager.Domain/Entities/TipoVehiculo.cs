using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class TipoVehiculo
    {
        public int Id { get; private set; }
        public string? Nombre { get; private set; }

        private TipoVehiculo() { }
        public TipoVehiculo(string nombre)
        {
            Nombre = nombre;
        }
    }
}