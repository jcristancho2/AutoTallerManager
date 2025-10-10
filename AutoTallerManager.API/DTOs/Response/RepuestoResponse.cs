using System;

namespace AutoTallerManager.API.DTOs.Response
{
    public record RepuestoResponse
    {
        public int RepuestoId { get; init; }
        public string? Codigo { get; init; }
        public string? NombreRepu { get; init; }
        public string? Descripcion { get; init; }
        public int Stock { get; init; }
        public decimal PrecioUnitario { get; init; }
        public int CategoriaId { get; init; }
        public int TipoVehiculoId { get; init; }
        public int FabricanteId { get; init; }
    }
}

