using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class ServiceStatus : BaseEntity
    {
        public string? ServiceStatusName { get; set; }
        public ICollection<ServiceOrder>? ServiceOrders { get; set; } 
    }
}