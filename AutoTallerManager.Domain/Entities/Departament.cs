using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class Departament : BaseEntity
    {
        public string? Name { get; set; }
        public int CountryId { get; set; }
        public Country Country { get; set; } = null!;

        private Departament() { }
        public Departament(string name, int country_Id)
        {
            Name = name;
            CountryId = country_Id;
        }

        public ICollection<City> Cities { get; set; } = new List<City>();

    }
}