using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configurations
{
    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("customers");

            // Clave primaria (GUID)
            builder.HasKey(c => c.Id)
                   .HasName("pk_cliente");

            builder.Property(c => c.Id)
                   .HasColumnName("id")
                   .ValueGeneratedOnAdd(); 

            // Propiedades básicas
            builder.Property(c => c.NombreCompleto)
                   .HasColumnName("nombre_completo")
                   .HasMaxLength(150)
                   .IsRequired();

            builder.Property(c => c.Telefono)
                   .HasColumnName("telefono")
                   .HasMaxLength(20)
                   .IsRequired(false);

            builder.Property(c => c.Email)
                   .HasColumnName("email")
                   .HasMaxLength(100)
                   .IsRequired(false);

            // Propiedades de relación (FKs)
            builder.Property(c => c.TipoCliente_Id)
                   .HasColumnName("tipo_cliente_id")
                   .IsRequired();

            builder.Property(c => c.Direccion_Id)
                   .HasColumnName("direccion_id")
                   .IsRequired();

            // Relaciones
            builder.HasOne<TipoCliente>()
                   .WithMany(t => t.Clientes)
                   .HasForeignKey(c => c.TipoCliente_Id)
                   .HasConstraintName("fk_cliente_tipo_cliente")
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Direccion>()
                   .WithMany(d => d.Clientes)
                   .HasForeignKey(c => c.Direccion_Id)
                   .HasConstraintName("fk_cliente_direccion")
                   .OnDelete(DeleteBehavior.Restrict);

            // // Índices opcionales
            // builder.HasIndex(c => c.Email)
            //        .HasDatabaseName("ix_cliente_Email");

            // builder.HasIndex(c => c.Telefono)
            //        .HasDatabaseName("ix_cliente_telefono");
        }
    }
}