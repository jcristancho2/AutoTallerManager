using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Domain.Entities
{
    public class ServiceOrder : BaseEntity
    {
        public int ServiceOrderId { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public string? WorkDescription { get; set; }
        public int VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }

        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }


        public int MechanicId { get; set; }  // Referencia a Usuario
        public User? Mechanic { get; set; }

        public int ServiceTypeId { get; set; }
        public ServiceType? ServiceType { get; set; }

        public int StatusId { get; set; }
        public ServiceStatus? ServiceStatus { get; set; }
        // Relaciones correctas
        public ICollection<OrderDetail>? OrderDetails { get; set; }
        public ICollection<Invoice>? Invoices { get; set; }
        //public Invoice? Invoice { get; set; }
    }
}