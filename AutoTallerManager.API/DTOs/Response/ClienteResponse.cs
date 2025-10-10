using System;
using System.Collections.Generic;

namespace AutoTallerManager.API.DTOs.Response
{
    public record ClienteResponse
    {
        public Guid Id { get; init; }
        public string? NombreCompleto { get; init; }
        public string? Telefono { get; init; }
        public string? Email { get; init; }
        public int TipoCliente_Id { get; init; }
        public int Direccion_Id { get; init; }

        public IReadOnlyCollection<VehiculoSummary>? Vehiculos { get; init; }

        public record VehiculoSummary
        {
            public int VehiculoId { get; init; }
            public string? Placa { get; init; }
            public int Anio { get; init; }
            public string? VIN { get; init; }
        }
    }
}