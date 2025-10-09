using System;
using System.ComponentModel.DataAnnotations;

namespace AutoTallerManager.API.DTOs.Auth;

public class LoginDto
{
    [Required]
    public string? Username { get; set; }
    [Required]
    public string? Password { get; set; }
}
