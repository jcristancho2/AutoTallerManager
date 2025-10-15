using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class OrderDetail : BaseEntity
    {
        public int DetailOrderId { get; set; }
        public int ServiceOrderId { get; set; }
        public int? SpareId { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LaborCost { get; set; }
        public ServiceOrder? ServiceOrder { get; set; }
        public Spare? Spare { get; set; }
    }
}