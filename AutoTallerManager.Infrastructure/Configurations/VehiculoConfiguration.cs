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
                   .WithMany(c => c.Vehiculos)
                   .HasForeignKey(v => v.ClienteId)
                   .HasConstraintName("FK_Vehiculos_Clientes_ClienteId")
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.TipoVehiculo)
                   .WithMany(tv => tv.Vehiculos)
                   .HasForeignKey(v => v.TipoVehiculoId)
                   .HasConstraintName("FK_Vehiculos_TiposVehiculo_TipoVehiculoId")
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.MarcaVehiculo)
                   .WithMany(mv => mv.Vehiculos)
                   .HasForeignKey(v => v.MarcaVehiculoId)
                   .HasConstraintName("FK_Vehiculos_MarcasVehiculo_MarcaVehiculoId")
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.ModeloVehiculo)
                   .WithMany(mv => mv.Vehiculos)
                   .HasForeignKey(v => v.ModeloVehiculoId)
                   .HasConstraintName("FK_Vehiculos_ModelosVehiculo_ModeloVehiculoId")
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}