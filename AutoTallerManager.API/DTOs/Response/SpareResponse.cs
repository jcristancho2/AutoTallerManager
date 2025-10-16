using System;

namespace AutoTallerManager.API.DTOs.Response
{
    public record SpareResponse
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Stock { get; set; }
        public decimal UnitPrice { get; set; }
        public int CategoryId { get; set; }
        public int VehicleTypeId { get; set; }
        public int ManufacturerId { get; set; }
    }
}

