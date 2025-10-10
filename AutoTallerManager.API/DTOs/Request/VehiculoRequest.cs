using System;
using System.ComponentModel.DataAnnotations;

namespace AutoTallerManager.API.DTOs.Request
{
    public record VehiculoRequest
    {
        [Required]
        [StringLength(10)]
        public string? Placa { get; init; }

        [Range(1900, 2100)]
        public int Anio { get; init; }

        [Required]
        [StringLength(30)]
        public string? VIN { get; init; }

        [Range(0, int.MaxValue)]
        public int Kilometraje { get; init; }

        [Required]
        public int ClienteId { get; init; }

        [Required]
        public int TipoVehiculoId { get; init; }

        [Required]
        public int MarcaVehiculoId { get; init; }

        [Required]
        public int ModeloVehiculoId { get; init; }
    }
}