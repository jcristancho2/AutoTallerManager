using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.API.Configuration
{
    public class AuditoriaConfiguration : IEntityTypeConfiguration<Auditoria>
    {
        public void Configure(EntityTypeBuilder<Auditoria> builder)
        {
            builder.ToTable("audits");

            // Clave primaria
            builder.HasKey(a => a.Id)
                   .HasName("pk_auditoria");

            builder.Property(a => a.Id)
                   .HasColumnName("id")
                   .ValueGeneratedOnAdd();

            // Propiedades
            builder.Property(a => a.UsuarioId)
                   .HasColumnName("usuario_id")
                   .IsRequired();

            builder.Property(a => a.EntidadAfectada)
                   .HasColumnName("entidad_afectada")
                   .HasMaxLength(150)
                   .IsRequired();

            builder.Property(a => a.AccionId)
                   .HasColumnName("accion_id")
                   .IsRequired();

            builder.Property(a => a.FechaHora)
                   .HasColumnName("fecha_hora")
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("CURRENT_TIMESTAMP")
                   .IsRequired();

            builder.Property(a => a.DescripcionAccion)
                   .HasColumnName("descripcion_accion")
                   .HasMaxLength(500)
                   .IsRequired(false);

            // Relaciones
            builder.HasOne<TipoAccion>()
                   .WithMany()
                   .HasForeignKey(a => a.AccionId)
                   .HasConstraintName("fk_auditoria_tipo_accion")
                   .OnDelete(DeleteBehavior.Restrict);


            // Índices útiles
            builder.HasIndex(a => a.FechaHora)
                   .HasDatabaseName("ix_auditoria_fecha_hora");

            builder.HasIndex(a => a.UsuarioId)
                   .HasDatabaseName("ix_auditoria_usuario_id");
        }
    }
}