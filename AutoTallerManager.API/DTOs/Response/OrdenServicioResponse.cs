using System;

namespace AutoTallerManager.API.DTOs.Response
{
    public record OrdenServicioResponse
    {
        public int OrdenServicioId { get; set; }
        public DateTime FechaIngreso { get; set; }
        public DateTime FechaEstimadaEntrega { get; set; }
        public int VehiculoId { get; set; }
        public int TipoServId { get; set; }
        public int EstadoId { get; set; }
    }
}