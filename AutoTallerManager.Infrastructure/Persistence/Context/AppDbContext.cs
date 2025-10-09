using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;             
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Infrastructure.Persistence.Context;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
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

    // public DbSet<Usuario> Usuarios { get; set; } = null!;
    // public DbSet<Rol> Roles { get; set; } = null!;
    // public DbSet<MarcaVehiculo> MarcasVehiculo { get; set;


    protected override void OnModelCreating(ModelBuilder modelBuilder) // <- ModelBuilder (no ModuleBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}
