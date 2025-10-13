using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configurations;

public class AuditoriaConfiguration : IEntityTypeConfiguration<Auditoria>
{
public void Configure(EntityTypeBuilder<Auditoria> builder)
{
       // Nombre de la tabla
       builder.ToTable("auditorias");

       // Clave primaria
       builder.HasKey(a => a.Id)
              .HasName("pk_auditoria");

       builder.Property(a => a.Id)
              .HasColumnName("auditoria_id")
              .ValueGeneratedOnAdd();

       // Propiedades
       builder.Property(a => a.UsuarioId)
              .HasColumnName("usuario_id")
              .IsRequired();

       builder.Property(a => a.EntidadAfectada)
              .HasColumnName("entidad_afectada")
              .HasMaxLength(50)
              .IsRequired();

       builder.Property(a => a.AccionId)
              .HasColumnName("accion_id")
              .IsRequired();

       builder.Property(a => a.FechaHora)
              .HasColumnName("fecha_hora")
              .HasDefaultValueSql("CURRENT_TIMESTAMP")
              .IsRequired();

       builder.Property(a => a.DescripcionAccion)
              .HasColumnName("descripcion_accion")
              .HasColumnType("TEXT");

       // Relaciones
       builder.HasOne(a => a.Usuario)
              .WithMany(u => u.Auditorias)
              .HasForeignKey(a => a.UsuarioId)
              .HasConstraintName("fk_auditoria_usuario")
              .OnDelete(DeleteBehavior.Restrict);

       builder.HasOne(a => a.TipoAccion)
              .WithMany() // TipoAccion no tiene colección de Auditorias (evita error)
              .HasForeignKey(a => a.AccionId)
              .HasConstraintName("fk_auditoria_tipo_accion")
              .OnDelete(DeleteBehavior.Restrict);

       // Índices
       builder.HasIndex(a => a.UsuarioId)
              .HasDatabaseName("ix_auditoria_usuario_id");

       builder.HasIndex(a => a.AccionId)
              .HasDatabaseName("ix_auditoria_accion_id");

       builder.HasIndex(a => a.FechaHora)
              .HasDatabaseName("ix_auditoria_fecha_hora");
}
}
