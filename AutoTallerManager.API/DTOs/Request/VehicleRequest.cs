using System;
using System.ComponentModel.DataAnnotations;

namespace AutoTallerManager.API.DTOs.Request
{
    public record VehicleRequest
    {
        [Required]
        public int VehicleId { get; set; }

        [Required]
        [StringLength(10)]
        public string? Plate { get; set; }

        [Range(1900, 2100)]
        public int Year { get; set; }

        [Required]
        [StringLength(30)]
        public string? VIN { get; set; }
        [Range(0, int.MaxValue)]
        public int Mileage { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int VehicleTypeId { get; set; }

        [Required]
        public int VehicleBrandId { get; set; }

        [Required]
        public int VehicleModelId { get; set; }
    }
}