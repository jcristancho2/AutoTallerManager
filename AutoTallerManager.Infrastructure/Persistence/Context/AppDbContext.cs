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
    public DbSet<Repuesto> Repuestos { get; set; } = null!;
    public DbSet<Categoria> Categorias { get; set; } = null!;
    public DbSet<TipoVehiculo> TiposVehiculo { get; set; } = null!;
    public DbSet<Fabricante> Fabricantes { get; set; } = null!;
    public DbSet<OrdenServicio> OrdenesServicio { get; set; } = null!;
    public DbSet<TipoServicio> TiposServicio { get; set; } = null!;
    public DbSet<Vehiculo> Vehiculos { get; set; } = null!;
    public DbSet<Factura> Facturas { get; set; } = null!;

    public DbSet<DetalleOrden> DetallesOrden { get; set; } = null!;
    public DbSet<TipoPago> TiposPago { get; set; } = null!;
    public DbSet<EstadoServ> EstadosServ { get; set; } = null!;

     public DbSet<Rol> Rols { get; set; } = null!;
    public DbSet<MarcaVehiculo> MarcasVehiculo { get; set; } = null!;

    // Auth & Identity DbSets - JWT

    public DbSet<UserMember> UsersMembers { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<UserMemberRol> UserMemberRols { get; set; } = null!;
    public DbSet<UserMemberRol> UsersMembersRols { get; set; } = null!;
    //


    public DbSet<Cliente> Clientes { get; set; } = null!;
    public DbSet<Departamento> Departamentos { get; set; } = null!;
    public DbSet<Direccion> Direcciones { get; set; } = null!;

    public DbSet<Auditoria> Auditorias { get; set; } = null!;

    public DbSet<BaseEntity> BaseEntities { get; set; } = null!;

    public DbSet<TipoAccion> TipoAcciones { get; set; } = null!;

    public DbSet<TipoPago> TipoPagos { get; set; } = null!;

    public DbSet<EstadoServ> EstadoServs { get; set; } = null!;
    
    public DbSet<Pais> Paises { get; set; } = null!;




    protected override void OnModelCreating(ModelBuilder modelBuilder) // <- ModelBuilder (no ModuleBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}
