using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;             
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Domain.Entities.Auth;

namespace AutoTallerManager.Infrastructure.Persistence.Context;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // DbSets
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }  
    public DbSet<EstadoUsuario> EstadosUsuario { get; set; } 
    public DbSet<Vehiculo> Vehiculos { get; set; }
    public DbSet<OrdenServicio> OrdenesServicio { get; set; }
    public DbSet<DetalleOrden> DetalleOrdenes { get; set; }
    public DbSet<Factura> Facturas { get; set; }
    public DbSet<Repuesto> Repuestos { get; set; }
    public DbSet<Auditoria> Auditorias { get; set; }

    // Auth & Identity DbSets - JWT

    public DbSet<UserMember> UsersMembers { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<UserMemberRol> UserMemberRols { get; set; } = null!;
    public DbSet<UserMemberRol> UsersMembersRols { get; set; } = null!;
    
    // Catálogos
    public DbSet<Rol> Roles { get; set; }
    public DbSet<TipoCliente> TiposCliente { get; set; }
    public DbSet<TipoVehiculo> TiposVehiculo { get; set; }
    public DbSet<MarcaVehiculo> MarcasVehiculo { get; set; }
    public DbSet<ModeloVehiculo> ModelosVehiculo { get; set; }
    public DbSet<TipoServicio> TiposServicio { get; set; }
    public DbSet<EstadoServ> EstadosServicio { get; set; }
    public DbSet<TipoPago> TiposPago { get; set; }
    public DbSet<TipoAccion> TiposAccion { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Fabricante> Fabricantes { get; set; }
    public DbSet<Pais> Paises { get; set; }
    public DbSet<Departamento> Departamentos { get; set; }
    public DbSet<Ciudad> Ciudades { get; set; }
    public DbSet<Direccion> Direcciones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configurar naming convention global para snake_case
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Convertir nombres de tablas a snake_case
            entity.SetTableName(ToSnakeCase(entity.GetTableName()));
            
            // Convertir nombres de columnas a snake_case
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(ToSnakeCase(property.GetColumnName()));
            }
            
            // Convertir nombres de claves foráneas a snake_case
            foreach (var key in entity.GetKeys())
            {
                key.SetName(ToSnakeCase(key.GetName()));
            }
            
            // Convertir nombres de índices a snake_case
            foreach (var index in entity.GetIndexes())
            {
                index.SetDatabaseName(ToSnakeCase(index.GetDatabaseName()));
            }
        }
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
    
    private static string ToSnakeCase(string? input)
    {
        if (string.IsNullOrEmpty(input)) return input ?? string.Empty;
        
        return string.Concat(input.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
    }
}
