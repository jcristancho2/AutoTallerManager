using System.ComponentModel.DataAnnotations;

namespace AutoTallerManager.API.DTOs.Auth;

public class CreateRoleDto
{
    [Required]
    [StringLength(50)]
    public string RoleName { get; set; } = string.Empty;

    [StringLength(200)]
    public string Description { get; set; } = string.Empty;
}
