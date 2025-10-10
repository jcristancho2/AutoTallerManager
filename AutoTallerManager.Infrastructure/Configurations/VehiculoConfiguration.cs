using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configuration
{
   public class VehiculoConfiguration : IEntityTypeConfiguration<Vehiculo>
    {
        public void Configure(EntityTypeBuilder<Vehiculo> builder)
        {
            builder.ToTable("vehicles");

            builder.HasKey(v => v.Id);
                builder.Property(v => v.Id)
                         .HasColumnName("vehiculo_id");

              builder.Property(v => v.ClienteId)
                     .HasColumnName("cliente_id")
                     .IsRequired();

              builder.Property(v => v.Placa)
                   .HasColumnName("placa")
                   .IsRequired()
                   .HasMaxLength(20);

                     builder.Property(v => v.Anio)
                     .HasColumnName("anio")
                   .IsRequired();
                

                 
              builder.Property(v => v.VIN) // esto es preferible eliminarlo porque con la placa es suficiente
                   .HasColumnName("vin")
                   .IsRequired()
                   .HasMaxLength(50);

              builder.Property(v => v.Kilometraje)
                   .HasColumnName("kilometraje")
                   .IsRequired();


            builder.HasIndex(v => v.Placa)
                   .IsUnique();

            builder.HasIndex(v => v.VIN)
                   .IsUnique();

            // Configurar tabla con check constraint
            builder.ToTable(t => t.HasCheckConstraint("ck_vehicle_kilometraje", "kilometraje >= 0"));

            // FKs y delete restrict
            builder.HasOne(v => v.Cliente)
                   .WithMany(c => c.Vehiculos)
                   .HasForeignKey(v => v.ClienteId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.TipoVehiculo)
                   .WithMany(tv => tv.Vehiculos)
                   .HasForeignKey(v => v.TipoVehiculoId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.MarcaVehiculo)
                   .WithMany(mv => mv.Vehiculos)
                   .HasForeignKey(v => v.MarcaVehiculoId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.ModeloVehiculo)
                   .WithMany(mv => mv.Vehiculos)
                   .HasForeignKey(v => v.ModeloVehiculoId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}