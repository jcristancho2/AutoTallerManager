using System;
using System.Collections.Generic;

namespace AutoTallerManager.API.DTOs.Response
{
    public record ClienteResponse
    {
        public int Id { get; set; }
        public string? NombreCompleto { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public int TipoCliente_Id { get; set; }
        public int Direccion_Id { get; set; }

        public IReadOnlyCollection<VehiculoSummary>? Vehiculos { get; set; }

        public record VehiculoSummary
        {
            public int VehiculoId { get; set; }
            public string? Placa { get; set; }
            public int Anio { get; set; }
            public string? VIN { get; set; }
        }
    }
}