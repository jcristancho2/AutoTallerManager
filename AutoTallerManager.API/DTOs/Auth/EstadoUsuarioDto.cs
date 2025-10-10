using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.API.DTOs.Auth;

public class EstadoUsuarioDto
{
    public int Id { get; set; }
    public string NombreEstUsu { get; set; } = string.Empty;
}
