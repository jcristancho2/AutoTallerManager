using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class Cliente
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string? NombreCompleto { get; private set; }
        public string? Telefono { get; private set; }
        public string? Correo { get; private set; }
        public int TipoCliente_Id { get; private set; }
        public int Direccion_Id { get; private set; }

        //constructor
        private Cliente() { }

        public Cliente(
            string nombreCompleto,
            string telefono,
            string correo,
            int tipoCliente_Id,
            int direccion_Id
            )
        {
            NombreCompleto = nombreCompleto;
            Telefono = telefono;
            Correo = correo;
            TipoCliente_Id = tipoCliente_Id;
            Direccion_Id = direccion_Id;
        }

        public ICollection<Vehiculo> Vehiculos { get; set; } = new HashSet<Vehiculo>();
        public ICollection<Factura> Facturas { get; set; } = new HashSet<Factura>();
    }
    

}