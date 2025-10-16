namespace AutoTallerManager.API.DTOs.Auth;

public class UseLoginResponseDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
}


