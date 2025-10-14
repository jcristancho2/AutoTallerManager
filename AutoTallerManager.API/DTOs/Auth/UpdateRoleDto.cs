using System.ComponentModel.DataAnnotations;

namespace AutoTallerManager.API.DTOs.Auth;

public class UpdateRoleDto
{
    [Required]
    [StringLength(50)]
    public string NombreRol { get; set; } = string.Empty;

    [StringLength(200)]
    public string Descripcion { get; set; } = string.Empty;
}
