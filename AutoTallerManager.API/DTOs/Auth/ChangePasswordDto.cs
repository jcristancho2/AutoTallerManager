using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.API.DTOs.Auth
{
    public class ChangePasswordDto
    {
        [Required]
        [MinLength(4, ErrorMessage = "La contraseña debe tener al menos 4 caracteres")]
        public string NewPassword { get; set; } = string.Empty;
    }
}