using System;
using System.ComponentModel.DataAnnotations;

namespace AutoTallerManager.API.DTOs.Request
{
    public record FacturaRequest
    {
        [Required]
        public DateTime Fecha { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Total { get; set; }

        [Required]
        public int OrdenServicioId { get; set; }

        [Required]
        public int ClienteId { get; set; }

        [Required]
        public int TipoPagoId { get; set; }
    }
}