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
    public DbSet<Customer> Customers { get; set; }
    public DbSet<User> Users { get; set; }  
    public DbSet<UserStatus> UserStatuses { get; set; } 
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<ServiceOrder> ServiceOrders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<SparePart> SpareParts { get; set; }
    public DbSet<Audit> Audits { get; set; }

    // Auth & Identity DbSets - JWT

    public DbSet<UserMember> UsersMembers { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<UserMemberRol> UserMemberRols { get; set; } = null!;
    public DbSet<UserMemberRol> UsersMembersRols { get; set; } = null!;
    
    // Catálogos
    public DbSet<Role> Roles { get; set; }
    public DbSet<CustomerType> CustomerTypes { get; set; }
    public DbSet<VehicleType> VehicleTypes { get; set; }
    public DbSet<VehicleBrand> VehicleBrands { get; set; }
    public DbSet<VehicleModel> VehicleModels { get; set; }
    public DbSet<ServiceType> ServiceTypes { get; set; }
    public DbSet<ServiceStatus> ServiceStatuses { get; set; }
    public DbSet<PaymentType> PaymentTypes { get; set; }
    public DbSet<ActionType> ActionTypes { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Manufacturer> Manufacturers { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<State> States { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Address> Addresses { get; set; }

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
