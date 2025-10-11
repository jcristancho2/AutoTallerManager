using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.API.DTOs.Auth;

public class UsuarioDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string RolNombre { get; set; } = string.Empty;
    public string EstadoNombre { get; set; } = "Activo";
}


