using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int Customer_Type_Id { get; set; }
        public int Address_Id { get; set; }

        public virtual Address Address { get; set; } = null!;
        public virtual CustomerType CustomerType { get; set; } = null!;
        public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}