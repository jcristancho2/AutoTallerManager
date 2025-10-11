using System;

namespace AutoTallerManager.API.DTOs.Response
{
    public record FacturaResponse
    {
        public int FacturaId { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public int OrdenServicioId { get; set; }
        public int ClienteId { get; set; }
        public int TipoPagoId { get; set; }
    }
}