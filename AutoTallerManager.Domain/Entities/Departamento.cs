using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class Departamento
    {
        public int Id { get; private set; }
        public string? Nombre { get; private set; }
        public int PaisId { get; private set; }
        public Pais Pais { get; private set; } = null!;

        private Departamento() { }
        public Departamento(string nombre, int paisId)
        {
            Nombre = nombre;
            PaisId = paisId;
        }

        public ICollection<Ciudad> Ciudades { get; set; } = new List<Ciudad>();
    }
}