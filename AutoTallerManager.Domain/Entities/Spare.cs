using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class Spare : BaseEntity
    {
        // Id inherited from BaseEntity
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Stock { get; set; }
        public decimal UnitPrice { get; set; }

        public int StockMin { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public int VehicleTypeId { get; set; }
        public VehicleType? VehicleType { get; set; }

        public int ManufacturerId { get; set; }
        public Manufacturer? Manufacturer { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }




    }
}