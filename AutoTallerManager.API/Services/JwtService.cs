using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoTallerManager.API.Helpers;
using AutoTallerManager.API.Services.Interfaces.Auth;
using AutoTallerManager.Domain.Entities.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AutoTallerManager.API.Services.Implementations.Auth
{
    public class JwtService : IJwtService
    {
        private readonly JWT _jwt;

        public JwtService(IOptions<JWT> jwt)
        {
            _jwt = jwt.Value;
        }

        public JwtSecurityToken CreateJwtToken(UserMember user)
        {
            var roleClaims = user.UserMemberRoles?
                .Where(umr => umr.Rol != null && umr.Rol!.NombreRol != null)
                .Select(umr => new Claim("roles", umr.Rol!.NombreRol!))
                .ToList() ?? new List<Claim>();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim("uid", user.Id.ToString())
            }.Union(roleClaims);

            var keyBytes = Encoding.UTF8.GetBytes(_jwt.Key ?? throw new InvalidOperationException("JWT Key cannot be null"));
            var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
                signingCredentials: creds
            );
        }

        public string WriteToken(JwtSecurityToken token)
        {
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshToken CreateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var generator = System.Security.Cryptography.RandomNumberGenerator.Create();
            generator.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                Expiries = DateTime.UtcNow.AddDays(10),
                CreatedDate = DateTime.UtcNow
            };
        }
    }
}