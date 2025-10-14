using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class Address : BaseEntity
    {
        public string? Description { get; set; }
        public int City_Id { get; set; }
        public City City { get; set; } = null!;

        private Address() { }
        public Address(
            string description,
            int city_Id
        )
        {
            Description = description;
            City_Id = city_Id;
        }

        public ICollection<Customer> Customers { get; set; } = new List<Customer>();
    }
}