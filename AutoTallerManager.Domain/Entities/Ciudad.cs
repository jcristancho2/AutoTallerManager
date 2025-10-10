using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class Ciudad : BaseEntity
    {
        public string? Nombre { get; set; }

        public int Departamento_Id { get; set; }
        public Departamento Departamento { get; set; } = null!;

        private Ciudad() { }

        public Ciudad(string nombre, int departamento_Id)
        {
            Nombre = nombre;
            Departamento_Id = departamento_Id;
        }

        public ICollection<Direccion> Direcciones { get; set; } = new List<Direccion>();

    }
}