using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class Cliente : BaseEntity
    {
        public string? NombreCompleto { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public int TipoCliente_Id { get; set; }
        public int Direccion_Id { get; set; }

        public virtual Direccion Direccion { get; set; } = null!;
        public virtual TipoCliente TipoCliente { get; set; } = null!;
        public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
        public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();
    }
}