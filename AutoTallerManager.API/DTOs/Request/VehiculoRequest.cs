using System;
using System.ComponentModel.DataAnnotations;

namespace AutoTallerManager.API.DTOs.Request
{
    public record VehiculoRequest
    {
        [Required]
        [StringLength(10)]
        public string? Placa { get; set; }

        [Range(1900, 2100)]
        public int Anio { get; set; }

        [Required]
        [StringLength(30)]
        public string? VIN { get; set; }
        [Range(0, int.MaxValue)]
        public int Kilometraje { get; set; }

        [Required]
        public int ClienteId { get; set; }

        [Required]
        public int TipoVehiculoId { get; set; }

        [Required]
        public int MarcaVehiculoId { get; set; }

        [Required]
        public int ModeloVehiculoId { get; set; }
    }
}