using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configuration
{
    public class DepartamentoConfiguration : IEntityTypeConfiguration<Departamento>
    {
        public void Configure(EntityTypeBuilder<Departamento> builder)
        {
            builder.ToTable("states");

             // PK
            builder.HasKey(d => d.Id)
                   .HasName("pk_departamento");

            builder.Property(d => d.Id)
                   .HasColumnName("id")
                   .ValueGeneratedOnAdd();

            // Propiedades
            builder.Property(d => d.Nombre)
                   .HasColumnName("nombre")
                   .HasMaxLength(150)
                   .IsRequired(false); // permite null

            builder.Property(d => d.PaisId)
                   .HasColumnName("pais_id")
                   .IsRequired();

            // Relación con Pais (N:1)
            builder.HasOne(d => d.Pais)
                   .WithMany(p => p.Departamentos) 
                   .HasForeignKey(d => d.PaisId)
                   .HasConstraintName("fk_departamento_pais")
                   .OnDelete(DeleteBehavior.Restrict);

            // Relación con Ciudad (1:N)
            builder.HasMany(d => d.Ciudades)
                   .WithOne(c => c.Departamento)
                   .HasForeignKey(c => c.Departamento_Id) 
                   .HasConstraintName("fk_ciudad_departamento")
                   .OnDelete(DeleteBehavior.Restrict);

            // Índices útiles
            builder.HasIndex(d => d.PaisId)
                   .HasDatabaseName("ix_departamento_pais_id");

            builder.HasIndex(d => d.Nombre)
                   .HasDatabaseName("ix_departamento_nombre");
        }
    }
}