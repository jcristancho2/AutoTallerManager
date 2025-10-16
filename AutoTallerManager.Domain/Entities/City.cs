using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class City: BaseEntity
    {
        public string? Name { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;

        private City() { }

        public City(string name, int departmentId)
        {
            Name = name;
            DepartmentId = departmentId;
        }

        public ICollection<Address> Addresses { get; set; } = new List<Address>();

    }
}