using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoTallerManager.API.DTOs.Auth;
using AutoTallerManager.API.Helpers;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Domain.Entities.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using AutoTallerManager.API.Services.Interfaces.Auth;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.API.Services.Implementations.Auth;

public class UserService : IUserService
{
    private readonly JWT _jwt;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<UserMember> _passwordHasher;
    public UserService(IOptions<JWT> jwt, IUnitOfWork unitOfWork, IPasswordHasher<UserMember> passwordHasher)
    {
        _jwt = jwt.Value;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }
    public async Task<string> RegisterAsync(RegisterDto registerDto)
    {
        var usuario = new UserMember
        {
            Username = registerDto.Username ?? throw new ArgumentNullException(nameof(registerDto.Username)),
            Email = registerDto.Email ?? throw new ArgumentNullException(nameof(registerDto.Email)),
            Password = registerDto.Password ?? throw new ArgumentNullException(nameof(registerDto.Password)),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        usuario.Password = _passwordHasher.HashPassword(usuario, registerDto.Password!);

        var usuarioExiste = _unitOfWork.UserMembers
                                    .Find(u => u.Username.ToLower() == registerDto.Username.ToLower())
                                    .FirstOrDefault();

        if (usuarioExiste == null)
        {
            var defaultRoleName = UserAuthorization.rol_default.ToString();
            var rolPredeterminado = _unitOfWork.Roles
                                    .Find(u => EF.Functions.ILike(u.NombreRol, defaultRoleName))
                                    .FirstOrDefault();
            if (rolPredeterminado == null)
            {
                try
                {
                    // Intenta crear el rol por defecto si no existe
                    var nuevoRol = new Rol
                    {
                        NombreRol = defaultRoleName,
                        Descripcion = "Default role"
                    };
                    await _unitOfWork.Roles.AddAsync(nuevoRol);
                    await _unitOfWork.SaveChanges();
                    rolPredeterminado = nuevoRol;
                }
                catch
                {
                rolPredeterminado = _unitOfWork.Roles
                                    .Find(u => EF.Functions.ILike(u.NombreRol, defaultRoleName))
                                    .FirstOrDefault();
                if (rolPredeterminado == null)
                {
                    return $"No se encontró ni pudo crearse el rol predeterminado '{defaultRoleName}'.";
                }
            }
            }
            try
            {
                usuario.UserMemberRoles.Add(new UserMemberRol { 
                    UserMemberId = usuario.Id, 
                    RolId = rolPredeterminado.Id 
                });
                await _unitOfWork.UserMembers.AddAsync(usuario);
                await _unitOfWork.SaveChanges();

                return $"El usuario  {registerDto.Username} ha sido registrado exitosamente";
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return $"Error: {message}";
            }
        }
        else
        {
            return $"El usuario con {registerDto.Username} ya se encuentra registrado.";
        }
    }

    // public async Task<DataUserDto> GetTokenAsync(LoginDto model)
    // {
    //     DataUserDto datosUsuarioDto = new DataUserDto();
    //     if (string.IsNullOrEmpty(model.Username))
    //     {
    //         datosUsuarioDto.IsAuthenticated = false;
    //         datosUsuarioDto.Message = "El nombre de usuario no puede ser nulo o vacío.";
    //         return datosUsuarioDto;
    //     }
    //     var usuario = await _unitOfWork.UserMembers
    //                 .GetByUserNameAsync(model.Username);

    //     if (usuario == null)
    //     {
    //         datosUsuarioDto.IsAuthenticated = false;
    //         datosUsuarioDto.Message = $"No existe ningún usuario con el username {model.Username}.";
    //         return datosUsuarioDto;
    //     }

    //     if (string.IsNullOrEmpty(model.Password))
    //     {
    //         datosUsuarioDto.IsAuthenticated = false;
    //         datosUsuarioDto.Message = $"La contraseña no puede ser nula o vacía para el usuario {usuario.Username}.";
    //         return datosUsuarioDto;
    //     }

    //     var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.Password, model.Password);

    //     if (resultado == PasswordVerificationResult.Success)
    //     {
    //         datosUsuarioDto.IsAuthenticated = true;
    //         JwtSecurityToken jwtSecurityToken = CreateJwtToken(usuario);
    //         datosUsuarioDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    //         datosUsuarioDto.Email = usuario.Email;
    //         datosUsuarioDto.UserName = usuario.Username;
    //         datosUsuarioDto.Roles = usuario.Rols
    //                                         .Select(u => u.Name)
    //                                         .ToList();

    //         if (usuario.RefreshTokens.Any(a => a.IsActive))
    //         {
    //             var activeRefreshToken = usuario.RefreshTokens.Where(a => a.IsActive == true).FirstOrDefault();
    //             if (activeRefreshToken != null)
    //             {
    //                 datosUsuarioDto.RefreshToken = activeRefreshToken.Token;
    //                 datosUsuarioDto.RefreshTokenExpiration = activeRefreshToken.Expires;
    //             }
    //             else
    //             {
    //                 // If no active refresh token is found, create a new one
    //                 var refreshToken = CreateRefreshToken();
    //                 datosUsuarioDto.RefreshToken = refreshToken.Token;
    //                 datosUsuarioDto.RefreshTokenExpiration = refreshToken.Expires;
    //                 usuario.RefreshTokens.Add(refreshToken);
    //                 await _unitOfWork.UserMembers.UpdateAsync(usuario);
    //                 await _unitOfWork.SaveChangesAsync();
    //             }
    //         }
    //         else
    //         {
    //             var refreshToken = CreateRefreshToken();
    //             datosUsuarioDto.RefreshToken = refreshToken.Token;
    //             datosUsuarioDto.RefreshTokenExpiration = refreshToken.Expires;
    //             usuario.RefreshTokens.Add(refreshToken);
    //             await _unitOfWork.UserMembers.UpdateAsync(usuario);
    //             await _unitOfWork.SaveChangesAsync();
    //         }

    //         return datosUsuarioDto;
    //     }
    //     datosUsuarioDto.IsAuthenticated = false;
    //     datosUsuarioDto.Message = $"Credenciales incorrectas para el usuario {usuario.Username}.";
    //     return datosUsuarioDto;
    // }
    public async Task<DataUserDto> GetTokenAsync(LoginDto model, CancellationToken ct = default)
    {
        var dto = new DataUserDto { IsAuthenticated = false };

        var username = model.Username?.Trim();
        var password = model.Password ?? string.Empty;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            dto.Message = "Usuario o contraseña inválidos.";
            return dto;
        }

        var usuario = await _unitOfWork.UserMembers.GetByUserNameAsync(username, ct);
        if (usuario is null)
        {
            dto.Message = "Usuario o contraseña inválidos.";
            return dto;
        }

        // ✅ CORREGIR: Verificar que Password no sea null
        var hashedPassword = usuario.Password ?? string.Empty;
        var verification = _passwordHasher.VerifyHashedPassword(usuario, hashedPassword, password);
        if (verification == PasswordVerificationResult.Failed)
        {
            dto.Message = "Usuario o contraseña inválidos.";
            return dto;
        }

        if (verification == PasswordVerificationResult.SuccessRehashNeeded)
        {
            usuario.Password = _passwordHasher.HashPassword(usuario, password);
            await _unitOfWork.UserMembers.UpdateAsync(usuario, ct);
        }

        // ✅ CORREGIR: usar 'usuario' y propiedades correctas
        var roles = usuario.UserMemberRoles?.Select(umr => umr.Rol?.NombreRol ?? "").ToList() ?? new List<string>();
        usuario.RefreshTokens ??= new List<RefreshToken>();

        // ✅ CORREGIR: Revisar si hay método ExecuteInTransactionAsync o usar SaveChanges normal
        try
        {
            // Revocar tokens activos
            foreach (var token in usuario.RefreshTokens.Where(t => t.Active))
            {
                token.Revoked = true;
                token.Active = false;
            }

            var refresh = CreateRefreshToken();
            usuario.RefreshTokens.Add(refresh);

            await _unitOfWork.UserMembers.UpdateAsync(usuario, ct);
            await _unitOfWork.SaveChanges(ct);

            // Generar JWT
            var jwt = CreateJwtToken(usuario);

            var currentRefresh = usuario.RefreshTokens.OrderByDescending(t => t.CreatedDate).First();

            dto.IsAuthenticated = true;
            dto.Token = new JwtSecurityTokenHandler().WriteToken(jwt);
            dto.Email = usuario.Email;
            dto.UserName = usuario.Username;
            dto.Roles = roles;
            dto.RefreshToken = currentRefresh.Token;
            dto.RefreshTokenExpiration = currentRefresh.Expiries;  // ✅ USAR ExpiryDate

            return dto;
        }
        catch (Exception ex)
        {
            dto.Message = $"Error interno: {ex.Message}";
            return dto;
        }
    }
    private JwtSecurityToken CreateJwtToken(UserMember usuario)
    {
        // ✅ CORREGIR: usar 'usuario' y propiedades correctas
        var userRoles = usuario.UserMemberRoles ?? new List<UserMemberRol>();
        var roleClaims = new List<Claim>();
        
        foreach (var userRole in userRoles)
        {
            if (userRole.Rol?.NombreRol != null)
            {
                roleClaims.Add(new Claim("roles", userRole.Rol.NombreRol));
            }
        }

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Username ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email ?? ""),
            new Claim("uid", usuario.Id.ToString())
        }
        .Union(roleClaims);

        if (string.IsNullOrEmpty(_jwt.Key))
        {
            throw new InvalidOperationException("JWT Key cannot be null or empty.");
        }

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        
        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
            signingCredentials: signingCredentials);
        
        return jwtSecurityToken;
    }
    private RefreshToken CreateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var generator = RandomNumberGenerator.Create())
        {
            generator.GetBytes(randomNumber);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                Expiries = DateTime.UtcNow.AddDays(10),  // ✅ USAR ExpiryDate
                Created = DateTime.UtcNow,             // ✅ USAR CreatedDate  
                Active = true,                           // ✅ AGREGAR IsActive
                Revoked = false                          // ✅ AGREGAR IsRevoked
            };
        }
    }
    public async Task<string> AddRoleAsync(AddRoleDto model)
    {
        if (string.IsNullOrEmpty(model.Username))
        {
            return "Username cannot be null or empty.";
        }

        var usuario = await _unitOfWork.UserMembers.GetByUserNameAsync(model.Username);  // ✅ CORREGIR
        if (usuario == null)
        {
            return $"User {model.Username} does not exists.";
        }

        if (string.IsNullOrEmpty(model.Password))
        {
            return $"Password cannot be null or empty.";
        }

        // ✅ CORREGIR: Verificar password con null-check
        var hashedPassword = usuario.Password ?? string.Empty;
        var result = _passwordHasher.VerifyHashedPassword(usuario, hashedPassword, model.Password);

        if (result == PasswordVerificationResult.Success)
        {
            if (string.IsNullOrWhiteSpace(model.Role))
            {
                return "Role name cannot be null or empty.";
            }

            var roleName = model.Role.Trim();

            // ✅ CORREGIR: usar NombreRol
            var rolExists = _unitOfWork.Roles
                                    .Find(u => EF.Functions.ILike(u.NombreRol, roleName))
                                    .FirstOrDefault();

            if (rolExists == null)
            {
                try
                {
                    var nuevoRol = new Rol
                    {
                        NombreRol = roleName,        // ✅ CORREGIR
                        Descripcion = $"{roleName} role"  // ✅ CORREGIR
                    };
                    await _unitOfWork.Roles.AddAsync(nuevoRol);
                    await _unitOfWork.SaveChanges();
                    rolExists = nuevoRol;
                }
                catch
                {
                    rolExists = _unitOfWork.Roles
                            .Find(u => EF.Functions.ILike(u.NombreRol, roleName))  // ✅ CORREGIR
                            .FirstOrDefault();
                    if (rolExists == null)
                    {
                        return $"No se encontró ni pudo crearse el rol '{roleName}'.";
                    }
                }
            }

            // ✅ CORREGIR: verificar si el usuario ya tiene el rol
            var userHasRole = usuario.UserMemberRoles?.Any(umr => 
                umr.Rol?.NombreRol != null && 
                umr.Rol.NombreRol.Equals(roleName, StringComparison.OrdinalIgnoreCase)) ?? false;

            if (!userHasRole)
            {
                // ✅ AGREGAR: relación correcta
                usuario.UserMemberRoles ??= new List<UserMemberRol>();
                usuario.UserMemberRoles.Add(new UserMemberRol
                {
                    UserMemberId = usuario.Id,
                    RolId = rolExists.Id,
                    Rol = rolExists
                });
                
                await _unitOfWork.UserMembers.UpdateAsync(usuario);
                await _unitOfWork.SaveChanges();
            }

            return $"Role {roleName} added to user {model.Username} successfully.";
        }
        return $"Invalid Credentials";
    }
    public async Task<DataUserDto> RefreshTokenAsync(string refreshToken)
    {
        var dataUserDto = new DataUserDto();

        var usuario = await _unitOfWork.UserMembers.GetByRefreshTokenAsync(refreshToken);

        if (usuario == null)
        {
            dataUserDto.IsAuthenticated = false;
            dataUserDto.Message = $"Token is not assigned to any user.";
            return dataUserDto;
        }

        var refreshTokenBd = usuario.RefreshTokens?.SingleOrDefault(x => x.Token == refreshToken);
        if (refreshTokenBd == null)
        {
            dataUserDto.IsAuthenticated = false;
            dataUserDto.Message = $"Token not found.";
            return dataUserDto;
        }

        // ✅ CORREGIR: usar IsActive correctamente
        if (!refreshTokenBd.IsActive || refreshTokenBd.IsRevoked)
        {
            dataUserDto.IsAuthenticated = false;
            dataUserDto.Message = $"Token is not active.";
            return dataUserDto;
        }

        // Revocar token actual
        refreshTokenBd.IsRevoked = true;
        refreshTokenBd.IsActive = false;

        // Crear nuevo refresh token
        var newRefreshToken = CreateRefreshToken();
        usuario.RefreshTokens ??= new List<RefreshToken>();
        usuario.RefreshTokens.Add(newRefreshToken);
        
        await _unitOfWork.UserMembers.UpdateAsync(usuario);
        await _unitOfWork.SaveChanges();

        // Generar nuevo JWT
        dataUserDto.IsAuthenticated = true;
        JwtSecurityToken jwtSecurityToken = CreateJwtToken(usuario);
        dataUserDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        dataUserDto.Email = usuario.Email;
        dataUserDto.UserName = usuario.Username;
        
        // ✅ CORREGIR: usar propiedades correctas
        dataUserDto.Roles = usuario.UserMemberRoles?
                                .Where(umr => umr.Rol != null)
                                .Select(umr => umr.Rol!.NombreRol)
                                .ToList() ?? new List<string>();
    
        dataUserDto.RefreshToken = newRefreshToken.Token;
        dataUserDto.RefreshTokenExpiration = newRefreshToken.Expired;  // ✅ USAR ExpiryDate

        return dataUserDto;
    }
}
