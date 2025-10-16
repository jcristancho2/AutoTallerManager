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
    public int RoleId { get; set; }
    [Required]
    public int StatusId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

}
