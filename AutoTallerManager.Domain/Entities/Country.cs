using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;


namespace AutoTallerManager.Domain.Entities
{
    public class Country : BaseEntity
    {
        public string? Name { get; set; }

        private Country() { }

        public Country(string name)
        {
            Name = name;
        }

        public ICollection<Departament> Departments { get; set; } = new List<Departament>();
    }
}