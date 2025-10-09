using System;
using AutoTallerManager.API.DTOs.Auth;

namespace AutoTallerManager.API.Services.Interfaces.Auth;

public interface IUserService
{
    Task<string> RegisterAsync(RegisterDto model);
    Task<DataUserDto> GetTokenAsync(LoginDto model, CancellationToken ct = default);

    Task<string> AddRoleAsync(AddRoleDto model);

    Task<DataUserDto> RefreshTokenAsync(string refreshToken);
}
