using System;
using System.ComponentModel.DataAnnotations;



namespace AutoTallerManager.API.DTOs.Request
{
    public record ClienteRequest
    {
        [Required]
        [StringLength(150)]
        public string? NombreCompleto { get; init; }

        [Required]
        [Phone]
        [StringLength(30)]
        public string? Telefono { get; init; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string? Email { get; init; }

        [Required]
        public int TipoCliente_Id { get; init; }

        [Required]
        public int Direccion_Id { get; init; }
    }
}