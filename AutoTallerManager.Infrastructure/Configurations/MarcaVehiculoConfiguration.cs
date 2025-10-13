using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configurations
{
    public class MarcaVehiculoConfiguration : IEntityTypeConfiguration<MarcaVehiculo>
    {
        public void Configure(EntityTypeBuilder<MarcaVehiculo> builder)
        {
            builder.ToTable("vehicle_brands");

            // Clave primaria
            builder.HasKey(m => m.Id)
                   .HasName("pk_marca_vehiculo");

            builder.Property(m => m.Id)
                   .HasColumnName("id")
                   .ValueGeneratedOnAdd();

            // Propiedades
            builder.Property(m => m.Nombre)
                   .HasColumnName("nombre")
                   .HasMaxLength(100)
                   .IsRequired();

            // La relación con Vehiculo se configura en VehiculoConfiguration

            // Índice opcional para búsquedas por nombre
            builder.HasIndex(m => m.Nombre)
                   .HasDatabaseName("ix_marca_vehiculo_nombre");
        }
    }
}