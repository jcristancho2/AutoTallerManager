using System;
using System.Collections.Generic;

namespace AutoTallerManager.API.DTOs.Response
{
    public record CustomerResponse
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int CustomerTypeId { get; set; }
        public int AddressId { get; set; }

        public IReadOnlyCollection<VehicleSummary>? Vehicle { get; set; }

        public record VehicleSummary
        {
            public int VehicleId { get; set; }
            public string? Plate { get; set; }
            public int Year { get; set; }
            public string? VIN { get; set; }
        }
    }
}