using System;

namespace AutoTallerManager.API.DTOs.Response
{
    public record VehiculoResponse
    {
        public int VehiculoId { get; init; }
        public string Placa { get; init; } = string.Empty;
        public int Anio { get; init; }
        public string VIN { get; init; } = string.Empty;
        public int Kilometraje { get; init; }

        public int ClienteId { get; init; }
        public int TipoVehiculoId { get; init; }
        public int MarcaVehiculoId { get; init; }
        public int ModeloVehiculoId { get; init; }
    }
}