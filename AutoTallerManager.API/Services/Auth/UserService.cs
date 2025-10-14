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
using AutoTallerManager.Infrastructure.Persistence.Context;

namespace AutoTallerManager.API.Services.Implementations.Auth
{
    public class UserService : IUserService
    {
        private readonly JWT _jwt;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<UserMember> _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly AppDbContext _context;

        public UserService(IOptions<JWT> jwt, IUnitOfWork unitOfWork, IPasswordHasher<UserMember> passwordHasher, IJwtService jwtService, AppDbContext context)
        {
            _jwt = jwt.Value;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _context = context;
        }

        public async Task<string> RegisterAsync(RegisterDto registerDto)
        {
            var usuario = new UserMember
            {
                Username = registerDto.Username ?? throw new ArgumentNullException(nameof(registerDto.Username)),
                Email = registerDto.Email ?? throw new ArgumentNullException(nameof(registerDto.Email)),
                Password = registerDto.Password ?? throw new ArgumentNullException(nameof(registerDto.Password)),
                CreatedAt = registerDto.CreatedAt,
                UpdatedAt = registerDto.UpdatedAt
            };

            usuario.Password = _passwordHasher.HashPassword(usuario, registerDto.Password!);

            var usuarioExiste = _unitOfWork.UserMembers
                                    .Find(u => u.Username != null && u.Username.ToLower() == registerDto.Username.ToLower())
                                    .FirstOrDefault();

            if (usuarioExiste != null)
                return $"El usuario {registerDto.Username} ya se encuentra registrado.";

            // Verificar si el rol especificado existe
            var rolAsignado = await _unitOfWork.Roles.GetByIdAsync(registerDto.RolId);
            if (rolAsignado == null)
            {
                // Si el rol no existe, usar rol por defecto
                var defaultRoleName = UserAuthorization.rol_default.ToString();
                rolAsignado = _unitOfWork.Roles
                                    .Find(u => u.NombreRol != null && EF.Functions.ILike(u.NombreRol, defaultRoleName))
                                    .FirstOrDefault();

                if (rolAsignado == null)
                {
                    var nuevoRol = new Rol
                    {
                        NombreRol = defaultRoleName,
                        Descripcion = "Default role"
                    };
                    await _unitOfWork.Roles.AddAsync(nuevoRol);
                    await _unitOfWork.SaveChanges();
                    rolAsignado = nuevoRol;
                }
            }

            // Usar el contexto directamente para manejar las relaciones
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Agregar el usuario
                await _context.UsersMembers.AddAsync(usuario);
                await _context.SaveChangesAsync();

                // Agregar la relación con el rol
                var userRole = new UserMemberRol
                {
                    UserMemberId = usuario.Id,
                    RolId = rolAsignado.Id
                };

                await _context.UserMemberRols.AddAsync(userRole);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            return $"El usuario {registerDto.Username} ha sido registrado exitosamente.";
        }

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
            
            // Asegurar que los roles estén cargados
            if (usuario != null && (usuario.UserMemberRoles == null || !usuario.UserMemberRoles.Any()))
            {
                // Recargar el usuario con roles incluidos
                usuario = await _unitOfWork.UserMembers.GetByIdAsync(usuario.Id, ct);
            }
            if (usuario == null)
            {
                dto.Message = "Usuario o contraseña inválidos.";
                return dto;
            }

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

            usuario.RefreshTokens ??= new List<RefreshToken>();
            foreach (var token in usuario.RefreshTokens.Where(t => t.IsActive))
            {
                token.Revoked = DateTime.UtcNow;
            }

            var newRefresh = CreateRefreshToken();
            usuario.RefreshTokens.Add(newRefresh);

            await _unitOfWork.UserMembers.UpdateAsync(usuario, ct);
            await _unitOfWork.SaveChanges(ct);

            var jwt = _jwtService.CreateJwtToken(usuario);

            dto.IsAuthenticated = true;
            dto.Token = new JwtSecurityTokenHandler().WriteToken(jwt);
            dto.Email = usuario.Email;
            dto.UserName = usuario.Username;
            dto.Roles = usuario.UserMemberRoles?
                                .Where(umr => umr.Rol != null && umr.Rol!.NombreRol != null)
                                .Select(umr => umr.Rol!.NombreRol!)
                                .ToList() ?? new List<string>();
            dto.RefreshToken = newRefresh.Token;
            dto.RefreshTokenExpiration = newRefresh.Expiries;

            return dto;
        }

        public async Task<string> AddRoleAsync(AddRoleDto model)
        {
            if (string.IsNullOrEmpty(model.Username))
                return "Username cannot be null or empty.";

            var usuario = await _unitOfWork.UserMembers.GetByUserNameAsync(model.Username);
            if (usuario == null)
                return $"User {model.Username} does not exist.";

            if (string.IsNullOrEmpty(model.Password))
                return "Password cannot be null or empty.";

            var hashedPassword = usuario.Password ?? string.Empty;
            var result = _passwordHasher.VerifyHashedPassword(usuario, hashedPassword, model.Password);
            if (result != PasswordVerificationResult.Success)
                return "Invalid credentials.";

            if (string.IsNullOrWhiteSpace(model.Role))
                return "Role name cannot be null or empty.";

            var roleName = model.Role.Trim();
            var rolExists = _unitOfWork.Roles
                                .Find(u => u.NombreRol != null && EF.Functions.ILike(u.NombreRol, roleName))
                                .FirstOrDefault();

            if (rolExists == null)
            {
                rolExists = new Rol { NombreRol = roleName, Descripcion = $"{roleName} role" };
                await _unitOfWork.Roles.AddAsync(rolExists);
                await _unitOfWork.SaveChanges();
            }

            usuario.UserMemberRoles ??= new List<UserMemberRol>();
            if (usuario.UserMemberRoles != null && !usuario.UserMemberRoles.Any(umr => umr.Rol?.NombreRol != null && umr.Rol.NombreRol.Equals(roleName, StringComparison.OrdinalIgnoreCase)))
            {
                usuario.UserMemberRoles.Add(new UserMemberRol { UserMemberId = usuario.Id, RolId = rolExists.Id, Rol = rolExists });
                await _unitOfWork.UserMembers.UpdateAsync(usuario);
                await _unitOfWork.SaveChanges();
            }

            return $"Role {roleName} added to user {model.Username} successfully.";
        }

        public async Task<DataUserDto> RefreshTokenAsync(string refreshToken)
        {
            var dataUserDto = new DataUserDto();
            var usuario = await _unitOfWork.UserMembers.GetByRefreshTokenAsync(refreshToken);

            if (usuario == null)
            {
                dataUserDto.IsAuthenticated = false;
                dataUserDto.Message = "Token no asignado a ningún usuario.";
                return dataUserDto;
            }

            var refreshTokenBd = usuario.RefreshTokens?.SingleOrDefault(x => x.Token == refreshToken);
            if (refreshTokenBd == null || !refreshTokenBd.IsActive || refreshTokenBd.IsRevoked)
            {
                dataUserDto.IsAuthenticated = false;
                dataUserDto.Message = "Token inválido o inactivo.";
                return dataUserDto;
            }

            refreshTokenBd.Revoked = DateTime.UtcNow;

            var newRefreshToken = CreateRefreshToken();
            usuario.RefreshTokens ??= new List<RefreshToken>();
            usuario.RefreshTokens.Add(newRefreshToken);

            await _unitOfWork.UserMembers.UpdateAsync(usuario);
            await _unitOfWork.SaveChanges();

            var jwtSecurityToken = _jwtService.CreateJwtToken(usuario);

            dataUserDto.IsAuthenticated = true;
            dataUserDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            dataUserDto.Email = usuario.Email;
            dataUserDto.UserName = usuario.Username;
            dataUserDto.Roles = usuario.UserMemberRoles?
                                .Where(umr => umr.Rol != null && umr.Rol!.NombreRol != null)
                                .Select(umr => umr.Rol!.NombreRol!)
                                .ToList() ?? new List<string>();
            dataUserDto.RefreshToken = newRefreshToken.Token;
            dataUserDto.RefreshTokenExpiration = newRefreshToken.Expiries;

            return dataUserDto;
        }

        private RefreshToken CreateRefreshToken()
        {
            return _jwtService.CreateRefreshToken();
        }
    }
}
