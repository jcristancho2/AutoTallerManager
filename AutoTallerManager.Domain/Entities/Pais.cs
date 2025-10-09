using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;


namespace AutoTallerManager.Domain.Entities
{
    public class Pais
    {
        public int Id { get; private set; }
        public string? Nombre { get; private set; }
        
        private Pais() { }

        public Pais(string nombre)
        {
            Nombre = nombre;
        }
    }
}