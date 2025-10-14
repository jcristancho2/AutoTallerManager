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
            builder.ToTable("vehiculos");

            builder.HasKey(v => v.Id);
            
            builder.Property(v => v.Id)
                   .HasColumnName("id")
                   .ValueGeneratedOnAdd();

            builder.Property(v => v.VIN)
                   .HasColumnName("vin")
                   .HasMaxLength(17);

            builder.Property(v => v.Anio)
                   .HasColumnName("anio")
                   .IsRequired();

            builder.Property(v => v.Kilometraje)
                   .HasColumnName("kilometraje")
                   .IsRequired();

            builder.Property(v => v.Placa)
                   .HasColumnName("placa")
                   .HasMaxLength(20);

            builder.Property(v => v.ClienteId)
                   .HasColumnName("cliente_id")
                   .IsRequired();

            builder.Property(v => v.TipoVehiculoId)
                   .HasColumnName("tipo_vehiculo_id")
                   .IsRequired();

            builder.Property(v => v.MarcaVehiculoId)
                   .HasColumnName("marca_vehiculo_id")
                   .IsRequired();

            builder.Property(v => v.ModeloVehiculoId)
                   .HasColumnName("modelo_vehiculo_id")
                   .IsRequired();

            builder.Property(v => v.CreatedAt)
                   .HasColumnName("created_at")
                   .IsRequired();

            builder.Property(v => v.UpdatedAt)
                   .HasColumnName("updated_at")
                   .IsRequired();

            // Índices únicos
            builder.HasIndex(v => v.VIN)
                   .IsUnique();

            builder.HasIndex(v => v.Placa)
                   .IsUnique();

            // Check constraints usando la nueva sintaxis
            builder.ToTable(t => t.HasCheckConstraint("CK_Vehiculo_Kilometraje", "kilometraje >= 0"));
            builder.ToTable(t => t.HasCheckConstraint("CK_Vehiculo_Anio", "anio >= 1900 AND anio <= 2030"));

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