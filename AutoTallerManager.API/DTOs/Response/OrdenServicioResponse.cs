using System;

namespace AutoTallerManager.API.DTOs.Response
{
    public record OrdenServicioResponse
    {
        public int OrdenServicioId { get; init; }
        public DateTime FechaIngreso { get; init; }
        public DateTime FechaEstimadaEntrega { get; init; }
        public int VehiculoId { get; init; }
        public int TipoServId { get; init; }
        public int EstadoId { get; init; }
    }
}