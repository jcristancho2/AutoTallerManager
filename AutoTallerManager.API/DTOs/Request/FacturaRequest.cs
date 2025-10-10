using System;
using System.ComponentModel.DataAnnotations;

namespace AutoTallerManager.API.DTOs.Request
{
    public record FacturaRequest
    {
        [Required]
        public DateTime Fecha { get; init; }

        [Range(0, double.MaxValue)]
        public decimal Total { get; init; }

        [Required]
        public int OrdenServicioId { get; init; }

        [Required]
        public int ClienteId { get; init; }

        [Required]
        public int TipoPagoId { get; init; }
    }
}