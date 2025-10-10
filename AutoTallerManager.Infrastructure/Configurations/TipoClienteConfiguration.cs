using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configuration
{
    public class TipoClienteConfiguration : IEntityTypeConfiguration<TipoCliente>
    {
        public void Configure(EntityTypeBuilder<TipoCliente> builder)
        {
           builder.ToTable("customers_types");

            // Clave primaria
            builder.HasKey(t => t.Id)
                   .HasName("pk_tipo_cliente");

            builder.Property(t => t.Id)
                   .HasColumnName("id")
                   .ValueGeneratedOnAdd();

            // Propiedades
            builder.Property(t => t.Nombre)
                   .HasColumnName("nombre")
                   .HasMaxLength(100)
                   .IsRequired();

            // Relación 1:N con Cliente
            builder.HasMany(t => t.Clientes)
                   .WithOne()
                   .HasForeignKey(c => c.TipoCliente_Id)
                   .HasConstraintName("fk_cliente_tipo_cliente")
                   .OnDelete(DeleteBehavior.Restrict);

            // Índice opcional para búsquedas o validaciones
            builder.HasIndex(t => t.Nombre)
                   .HasDatabaseName("ix_tipo_cliente_nombre");
        }
    }
}