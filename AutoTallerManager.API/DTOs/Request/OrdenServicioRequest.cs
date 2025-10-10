using System;
using System.ComponentModel.DataAnnotations;

namespace AutoTallerManager.API.DTOs.Request
{
    public record OrdenServicioRequest
    {
        [Required]
        public DateTime FechaIngreso { get; init; }

        [Required]
        public DateTime FechaEstimadaEntrega { get; init; }

        [Required]
        public int VehiculoId { get; init; }

        [Required]
        public int TipoServId { get; init; }

        [Required]
        public int EstadoId { get; init; }
    }
}