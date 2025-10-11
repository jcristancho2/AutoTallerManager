using System;

namespace AutoTallerManager.API.DTOs.Response
{
    public record RepuestoResponse
    {
        public int RepuestoId { get; set; }
        public string? Codigo { get; set; }
        public string? NombreRepu { get; set; }
        public string? Descripcion { get; set; }
        public int Stock { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int CategoriaId { get; set; }
        public int TipoVehiculoId { get; set; }
        public int FabricanteId { get; set; }
    }
}

