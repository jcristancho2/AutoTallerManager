using System.ComponentModel.DataAnnotations;

namespace AutoTallerManager.API.DTOs.Auth;

public class AssignRoleDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int RoleId { get; set; }
}
