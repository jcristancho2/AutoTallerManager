using System;
using System.ComponentModel.DataAnnotations;

namespace AutoTallerManager.API.DTOs.Request
{
    public record RepuestoRequest
    {
        [Required]
        [StringLength(30)]
        public string? Codigo { get; set; }

        [Required]
        [StringLength(150)]
        public string? NombreRepu { get; set; }

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PrecioUnitario { get; set; }

        [Required]
        public int CategoriaId { get; set; }

        [Required]
        public int TipoVehiculoId { get; set; }

        [Required]
        public int FabricanteId { get; set; }
    }
}