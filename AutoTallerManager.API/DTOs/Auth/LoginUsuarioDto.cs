using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AutoTallerManager.API.DTOs.Auth
{
    public class LoginUsuarioDto
    {
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;
}

    // moved to UsuarioLoginResponseDto.cs
}