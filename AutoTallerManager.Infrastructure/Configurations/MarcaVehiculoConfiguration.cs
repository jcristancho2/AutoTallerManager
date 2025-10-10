using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configuration
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

            // Relación 1:N con Vehiculo
            builder.HasMany(m => m.Vehiculos)
                   .WithOne(v => v.MarcaVehiculo)
                   .HasForeignKey(v => v.MarcaVehiculoId)
                   .HasConstraintName("fk_vehiculo_marca")
                   .OnDelete(DeleteBehavior.Restrict);

            // Índice opcional para búsquedas por nombre
            builder.HasIndex(m => m.Nombre)
                   .HasDatabaseName("ix_marca_vehiculo_nombre");
        }
    }
}