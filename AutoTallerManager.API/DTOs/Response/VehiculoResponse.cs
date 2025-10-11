using System;

namespace AutoTallerManager.API.DTOs.Response
{
    public record VehiculoResponse
    {
        public int VehiculoId { get; set; }
        public string Placa { get; set; } = string.Empty;
        public int Anio { get; set; }
        public string VIN { get; set; } = string.Empty;
        public int Kilometraje { get; set; }

        public int ClienteId { get; set; }
        public int TipoVehiculoId { get; set; }
        public int MarcaVehiculoId { get; set; }
        public int ModeloVehiculoId { get; set; }
    }
}