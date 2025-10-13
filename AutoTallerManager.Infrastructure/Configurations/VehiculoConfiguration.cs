using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configurations
{
    public class VehiculoConfiguration : IEntityTypeConfiguration<Vehiculo>
    {
        public void Configure(EntityTypeBuilder<Vehiculo> builder)
        {
            builder.ToTable("Vehiculos");

            builder.HasKey(v => v.Id);
            
            builder.Property(v => v.Id)
                   .HasColumnName("Id")
                   .ValueGeneratedOnAdd();

            builder.Property(v => v.VIN)
                   .HasColumnName("VIN")
                   .HasMaxLength(17);

            builder.Property(v => v.Anio)
                   .HasColumnName("Anio")
                   .IsRequired();

            builder.Property(v => v.Kilometraje)
                   .HasColumnName("Kilometraje")
                   .IsRequired();

            builder.Property(v => v.Placa)
                   .HasColumnName("Placa")
                   .HasMaxLength(20);

            builder.Property(v => v.ClienteId)
                   .HasColumnName("ClienteId")
                   .IsRequired();

            builder.Property(v => v.TipoVehiculoId)
                   .HasColumnName("TipoVehiculoId")
                   .IsRequired();

            builder.Property(v => v.MarcaVehiculoId)
                   .HasColumnName("MarcaVehiculoId")
                   .IsRequired();

            builder.Property(v => v.ModeloVehiculoId)
                   .HasColumnName("ModeloVehiculoId")
                   .IsRequired();

            builder.Property(v => v.CreatedAt)
                   .HasColumnName("CreatedAt")
                   .IsRequired();

            builder.Property(v => v.UpdatedAt)
                   .HasColumnName("UpdatedAt")
                   .IsRequired();

            // Índices únicos
            builder.HasIndex(v => v.VIN)
                   .IsUnique();

            builder.HasIndex(v => v.Placa)
                   .IsUnique();

            // Check constraints usando la nueva sintaxis
            builder.ToTable(t => t.HasCheckConstraint("CK_Vehiculo_Kilometraje", "Kilometraje >= 0"));
            builder.ToTable(t => t.HasCheckConstraint("CK_Vehiculo_Anio", "Anio >= 1900 AND Anio <= 2030"));

            // Relaciones
            builder.HasOne(v => v.Cliente)
                   .WithMany()
                   .HasForeignKey(v => v.ClienteId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.TipoVehiculo)
                   .WithMany()
                   .HasForeignKey(v => v.TipoVehiculoId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.MarcaVehiculo)
                   .WithMany()
                   .HasForeignKey(v => v.MarcaVehiculoId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.ModeloVehiculo)
                   .WithMany()
                   .HasForeignKey(v => v.ModeloVehiculoId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}