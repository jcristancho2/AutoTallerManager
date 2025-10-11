using System;
using System.ComponentModel.DataAnnotations;

namespace AutoTallerManager.API.DTOs.Auth;

public class RegisterDto
{
    [Required]
    public string? Email { get; set; }
    [Required]
    public string? Username { get; set; }
    [Required]
    public string? Password { get; set; }
    [Required]
    public int RolId { get; set; }
    [Required]
    public int EstadoId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

}
