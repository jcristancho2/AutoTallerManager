using System;

namespace AutoTallerManager.API.DTOs.Response
{
    public record ServiceOrderResponse
    {
        public int ServiceOrderId { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public int VehicleId { get; set; }
        public int ServiceTypeId { get; set; }
        public int StatusId { get; set; }
    }
}