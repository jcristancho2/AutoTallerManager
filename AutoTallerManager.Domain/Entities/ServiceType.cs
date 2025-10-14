using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class ServiceType : BaseEntity
    {
        public string? ServiceTypeName { get; set; }
        public ICollection<ServiceOrder>? ServiceOrders { get; set; }


    }
}