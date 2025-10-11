namespace AutoTallerManager.API.DTOs.Auth;

public class UsuarioLoginResponseDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string RolNombre { get; set; } = string.Empty;
    public string EstadoNombre { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
}


