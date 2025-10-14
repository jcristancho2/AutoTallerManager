using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class CustomerType: BaseEntity
    {
        public string? Name {get; set;} //persona natural o empresa
        private CustomerType() { }
        public CustomerType(string name)
        {
            Name = name;
        }

        public ICollection<Customer> Customers { get; set; } = new List<Customer>();
    }
}