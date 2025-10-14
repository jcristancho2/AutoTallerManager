using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class PaymentType : BaseEntity
    {
        public string? Name { get; set; }
        public ICollection<Invoice>? Invoices { get; set; }
    }
}