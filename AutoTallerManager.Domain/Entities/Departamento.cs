using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class Departamento : BaseEntity
    {
        public string? Nombre { get; set; }
        public int PaisId { get; set; }
        public Pais Pais { get; set; } = null!;

        private Departamento() { }
        public Departamento(string nombre, int paisId)
        {
            Nombre = nombre;
            PaisId = paisId;
        }

        public ICollection<Ciudad> Ciudades { get; set; } = new List<Ciudad>();
    }
}