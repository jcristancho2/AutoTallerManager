using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class City: BaseEntity
    {
        public string? Name { get; set; }

        public int State_Id { get; set; }
        public Department DepartmentId { get; set; } = null!;

        private City() { }

        public City(string name, int state_Id)
        {
            Name = name;
            State_Id = state_Id;
        }

        public ICollection<Address> Addresses { get; set; } = new List<Address>();

    }
}