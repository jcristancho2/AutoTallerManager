using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AutoTallerManager.API.DTOs.Auth
{
    public class CreateUsuarioDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public int RolId { get; set; }

        [Required]
        public int EstadoUsuarioId { get; set; }
    }   
}