using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configurations
{
    public class VehicleBrandConfiguration : IEntityTypeConfiguration<VehicleBrand>
    {
        public void Configure(EntityTypeBuilder<VehicleBrand> builder)
        {
            builder.ToTable("vehicle_brands");

            // Clave primaria
            builder.HasKey(m => m.Id)
                   .HasName("pk_vehicle_brand");

            builder.Property(m => m.Id)
                   .HasColumnName("id")
                   .ValueGeneratedOnAdd();

            // Propiedades
            builder.Property(m => m.Name)
                   .HasColumnName("name")
                   .HasMaxLength(100)
                   .IsRequired();

            // La relación con Vehiculo se configura en VehiculoConfiguration

            // Índice opcional para búsquedas por nombre
            builder.HasIndex(m => m.Name)
                   .HasDatabaseName("ix_vehicle_brand_name");
        }
    }
}