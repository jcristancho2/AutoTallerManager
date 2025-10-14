using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class Spare : BaseEntity
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Stock { get; set; }
        public decimal Unit_Price { get; set; }

        public int Stock_Min { get; set; }
        public int Category_Id { get; set; }
        public Category? Category { get; set; }

        public int Vehicle_Type_Id { get; set; }
        public VehicleType? VehicleType { get; set; }

        public int Manufacturer_Id { get; set; }
        public Manufacturer? Manufacturer { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }




    }
}