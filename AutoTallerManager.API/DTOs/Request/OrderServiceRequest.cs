using System;
using System.ComponentModel.DataAnnotations;

namespace AutoTallerManager.API.DTOs.Request
{
    public record ServiceOrderRequest
    {
        [Required]
        public DateTime EntryDate { get; set; }

        [Required]
        public DateTime EstimatedDeliveryDate { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [Required]
        public int ServiceTypeId { get; set; }

        [Required]
        public int StatusId { get; set; }
    }
}