using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configuration
{
    public class TipoAccionConfiguration : IEntityTypeConfiguration<TipoAccion>
    {
        public void Configure(EntityTypeBuilder<TipoAccion> builder)
        {
            builder.ToTable("types_actions");

            // Clave primaria
            builder.HasKey(t => t.Id)
                   .HasName("pk_tipo_accion");

            builder.Property(t => t.Id)
                   .HasColumnName("id")
                   .ValueGeneratedOnAdd();

            // Propiedades
            builder.Property(t => t.NombreAccion)
                   .HasColumnName("nombre_accion")
                   .HasMaxLength(100)
                   .IsRequired();

            // Índice opcional para búsquedas
            builder.HasIndex(t => t.NombreAccion)
                   .HasDatabaseName("ix_tipo_accion_nombre_accion");
        }
    }
 }