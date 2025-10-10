using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;


namespace AutoTallerManager.Domain.Entities
{
    public class Pais : BaseEntity
    {
        public string? Nombre { get; set; }
        
        private Pais() { }

        public Pais(string nombre)
        {
            Nombre = nombre;
        }

        public ICollection<Departamento> Departamentos { get; set; } = new List<Departamento>();
    }
}