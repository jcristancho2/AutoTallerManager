using System.IdentityModel.Tokens.Jwt;
using AutoTallerManager.Domain.Entities.Auth;

namespace AutoTallerManager.API.Services.Interfaces.Auth;

public interface IJwtService
{
    JwtSecurityToken CreateJwtToken(UserMember user);
    string WriteToken(JwtSecurityToken token);
    RefreshToken CreateRefreshToken();
}


