using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configuration
{
    public class PaisConfiguration : IEntityTypeConfiguration<Pais>
    {
        public void Configure(EntityTypeBuilder<Pais> builder)
        {
            builder.ToTable("countries");

            // Clave primaria
            builder.HasKey(p => p.Id)
                   .HasName("pk_pais");

            builder.Property(p => p.Id)
                   .HasColumnName("id")
                   .ValueGeneratedOnAdd();

            // Propiedades
            builder.Property(p => p.Nombre)
                   .HasColumnName("nombre")
                   .HasMaxLength(150)
                   .IsRequired();

            // Relación 1:N con Departamentos
            builder.HasMany(p => p.Departamentos)
                   .WithOne(d => d.Pais)
                   .HasForeignKey(d => d.PaisId)
                   .HasConstraintName("fk_departamento_pais")
                   .OnDelete(DeleteBehavior.Restrict);

            // Índice opcional por nombre
            builder.HasIndex(p => p.Nombre)
                   .HasDatabaseName("ix_pais_nombre");
        }
    }
}