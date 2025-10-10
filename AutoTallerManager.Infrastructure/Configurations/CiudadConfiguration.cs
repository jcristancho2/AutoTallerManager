using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configuration
{
    public class CiudadConfiguration : IEntityTypeConfiguration<Ciudad>
    {
        public void Configure(EntityTypeBuilder<Ciudad> builder)
        {
            builder.ToTable("cities");

             // Clave primaria
            builder.HasKey(c => c.Id)
                   .HasName("pk_ciudad");

            builder.Property(c => c.Id)
                   .HasColumnName("id")
                   .ValueGeneratedOnAdd();

            // Propiedades
            builder.Property(c => c.Nombre)
                   .HasColumnName("nombre")
                   .HasMaxLength(150)
                   .IsRequired(false); // el modelo permite null

            builder.Property(c => c.Departamento_Id)
                   .HasColumnName("departamento_id")
                   .IsRequired();

            // Relación con Departamento (N:1)
            builder.HasOne(c => c.Departamento)
                   .WithMany(d => d.Ciudades) 
                   .HasForeignKey(c => c.Departamento_Id)
                   .HasConstraintName("fk_ciudad_departamento")
                   .OnDelete(DeleteBehavior.Restrict);

            // Relación con Direcciones (1:N)
            builder.HasMany(c => c.Direcciones)
                   .WithOne(d => d.Ciudad)
                   .HasForeignKey(d => d.CiudadId)
                   .HasConstraintName("fk_direccion_ciudad")
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}