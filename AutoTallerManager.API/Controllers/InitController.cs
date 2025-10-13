using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Domain.Entities.Auth;
using Microsoft.AspNetCore.Identity;
using AutoTallerManager.Infrastructure.Persistence.Context;

namespace AutoTallerManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
 public class InitController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<UserMember> _passwordHasher;
    private readonly AppDbContext _db;

    public InitController(IUnitOfWork unitOfWork, IPasswordHasher<UserMember> passwordHasher, AppDbContext db)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _db = db;
    }

    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok(new { message = "InitController funciona correctamente" });
    }

    [HttpPost("create-admin")]
    public async Task<IActionResult> CreateAdmin()
    {
        try
        {
            // Verificar si el usuario ya existe
            var existingUser = await _unitOfWork.UserMembers.GetByUserNameAsync("admin@taller.com");
            if (existingUser != null)
            {
                return Ok(new { 
                    message = "Usuario admin ya existe",
                    email = "admin@taller.com"
                });
            }

            // Crear usuario admin
            var adminUser = new UserMember
            {
                Username = "admin@taller.com",
                Email = "admin@taller.com",
                Password = "Admin123!",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            adminUser.Password = _passwordHasher.HashPassword(adminUser, "Admin123!");

            await _unitOfWork.UserMembers.AddAsync(adminUser);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new { 
                message = "Usuario admin creado exitosamente",
                email = "admin@taller.com",
                password = "Admin123!",
                userId = adminUser.Id
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    [HttpPost("seed-basic")]
    public async Task<IActionResult> SeedBasic()
    {
        try
        {
            // Tipos de Cliente
            if (!_db.TiposCliente.Any())
            {
                _db.TiposCliente.AddRange(new TipoCliente("Particular"), new TipoCliente("Empresa"));
            }

            // Tipos de Vehículo
            if (!_db.TiposVehiculo.Any())
            {
                _db.TiposVehiculo.AddRange(new TipoVehiculo { NombreTipoVehiculo = "Automóvil" }, new TipoVehiculo { NombreTipoVehiculo = "Motocicleta" });
            }

            // Marcas / Modelos
            if (!_db.MarcasVehiculo.Any())
            {
                var toyota = new MarcaVehiculo { Nombre = "Toyota" };
                _db.MarcasVehiculo.Add(toyota);
                await _db.SaveChangesAsync();
                _db.ModelosVehiculo.Add(new ModeloVehiculo("Corolla"));
            }

            // Categorías y Fabricantes
            if (!_db.Categorias.Any())
            {
                _db.Categorias.Add(new Categoria { NombreCat = "Filtros" });
            }
            if (!_db.Fabricantes.Any())
            {
                _db.Fabricantes.Add(new Fabricante { NombreFab = "Bosch", Descripcion = "Repuestos", Telefono = "000", Email = "info@bosch.com" });
            }

            // Tipos de Servicio / Estados / Tipos de Pago
            if (!_db.TiposServicio.Any())
            {
                _db.TiposServicio.Add(new TipoServicio { NombreTipoServ = "Mantenimiento Preventivo" });
            }
            if (!_db.EstadosServicio.Any())
            {
                _db.EstadosServicio.AddRange(new EstadoServ { NombreEstServ = "Pendiente" }, new EstadoServ { NombreEstServ = "En Proceso" }, new EstadoServ { NombreEstServ = "Completada" });
            }
            if (!_db.TiposPago.Any())
            {
                _db.TiposPago.AddRange(new TipoPago { NombreTipoPag = "Efectivo" }, new TipoPago { NombreTipoPag = "Tarjeta" });
            }

            await _db.SaveChangesAsync();
            return Ok(new { message = "Seed básico aplicado" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
