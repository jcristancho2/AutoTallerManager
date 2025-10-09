using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class Ciudad
    {
        public int Id { get; private set; }
        public string? Nombre { get; private set; }

        public int Departamento_Id { get; private set; }
        public Departamento Departamento { get; private set; } = null!;

        private Ciudad() { }

        public Ciudad(string nombre, int departamento_Id)
        {
            Nombre = nombre;
            Departamento_Id = departamento_Id;
        }

        public ICollection<Direccion> Direcciones { get; set; } = new List<Direccion>();

    }
}