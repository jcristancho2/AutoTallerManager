using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class Direccion : BaseEntity
    {
        public string? Descripcion { get; set; }
        public int CiudadId { get; set; }
        public Ciudad Ciudad { get; set; } = null!;

        private Direccion() { }
        public Direccion(
            string descripcion,
            int ciudadId
        )
        {
            Descripcion = descripcion;
            CiudadId = ciudadId;
        }

        public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
    }
}