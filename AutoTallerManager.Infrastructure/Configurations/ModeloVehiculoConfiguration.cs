using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configurations
{
    public class ModeloVehiculoConfiguration : IEntityTypeConfiguration<ModeloVehiculo>
    {
        public void Configure(EntityTypeBuilder<ModeloVehiculo> builder)
        {
            builder.ToTable("vehicle_models");

            builder.HasKey(m => m.Id)
                   .HasName("pk_modelo_vehiculo");

            builder.Property(m => m.Id)
                   .HasColumnName("id")
                   .ValueGeneratedOnAdd();


            builder.Property(m => m.Nombre)
                   .HasColumnName("nombre")
                   .IsRequired()
                   .HasMaxLength(100);

        
        }
    }
}