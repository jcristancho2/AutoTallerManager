using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class Address : BaseEntity
    {
        public string? Description { get; set; }
        public int CityId { get; set; }
        public City City { get; set; } = null!;

        private Address() { }
        public Address(
            string description,
            int cityId
        )
        {
            Description = description;
            CityId = cityId;
        }

        public ICollection<Customer> Customers { get; set; } = new List<Customer>();
    }
}