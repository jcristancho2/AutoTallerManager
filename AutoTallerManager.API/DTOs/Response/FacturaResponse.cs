using System;

namespace AutoTallerManager.API.DTOs.Response
{
    public record FacturaResponse
    {
        public int FacturaId { get; init; }
        public DateTime Fecha { get; init; }
        public decimal Total { get; init; }
        public int OrdenServicioId { get; init; }
        public int ClienteId { get; init; }
        public int TipoPagoId { get; init; }
    }
}