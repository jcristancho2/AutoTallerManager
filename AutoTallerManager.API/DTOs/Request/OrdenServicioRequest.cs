using System;
using System.ComponentModel.DataAnnotations;

namespace AutoTallerManager.API.DTOs.Request
{
    public record OrdenServicioRequest
    {
        [Required]
        public DateTime FechaIngreso { get; set; }

        [Required]
        public DateTime FechaEstimadaEntrega { get; set; }

        [Required]
        public int VehiculoId { get; set; }

        [Required]
        public int TipoServId { get; set; }

        [Required]
        public int EstadoId { get; set; }
    }
}