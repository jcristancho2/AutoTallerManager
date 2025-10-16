using System;

namespace AutoTallerManager.API.DTOs.Response
{
    public record VehicleResponse
    {
        public int VehicleId { get; set; }
        public string Plate { get; set; } = string.Empty;
        public int Year { get; set; }
        public string VIN { get; set; } = string.Empty;
        public int Mileage { get; set; }

        public int CustomerId { get; set; }
        public int VehicleTypeId { get; set; }
        public int VehicleBrandId { get; set; }
        public int VehicleModelId { get; set; }
    }
}