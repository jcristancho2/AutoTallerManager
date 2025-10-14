using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class Vehicle : BaseEntity
    {
        public string? LicensePlate { get; set; }
        public int Year { get; set; }             
        public string? VIN { get; set; } // PILAS A ELIMINAR ESTO, ESTO NO SE VA A USAR
        public int Mileage { get; set; }

        public int Customer_id { get; set; }
        public Customer? Customer { get; set; }

        public int Vehicle_Type_Id { get; set; }
        public VehicleType? VehicleType { get; set; }

        public int VehicleBrandId { get; set; }
        public VehicleBrand? VehicleBrand { get; set; }

        public int VehicleModelId { get; set; }
        public VehicleModel? VehicleModel { get; set; }

        // Relación inversa típica (si usas OrdenServicio)
        public ICollection<ServiceOrder>? ServiceOrders { get; set; }
    }
}