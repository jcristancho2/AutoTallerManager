using System;
using System.ComponentModel.DataAnnotations;

namespace AutoTallerManager.API.DTOs.Request
{
    public record RepuestoRequest
    {
        [Required]
        [StringLength(30)]
        public string? Codigo { get; init; }

        [Required]
        [StringLength(150)]
        public string? NombreRepu { get; init; }

        [StringLength(500)]
        public string? Descripcion { get; init; }

        [Range(0, int.MaxValue)]
        public int Stock { get; init; }

        [Range(0, double.MaxValue)]
        public decimal PrecioUnitario { get; init; }

        [Required]
        public int CategoriaId { get; init; }

        [Required]
        public int TipoVehiculoId { get; init; }

        [Required]
        public int FabricanteId { get; init; }
    }
}