using System;
using System.ComponentModel.DataAnnotations;

namespace AutoTallerManager.API.DTOs.Request
{
    public record SpareRequest
    {
        [Required]
        [StringLength(30)]
        public string? Code { get; set; }

        [Required]
        [StringLength(150)]
        public string? Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int VehicleTypeId { get; set; }

        [Required]
        public int ManufacturerId { get; set; }
    }
}