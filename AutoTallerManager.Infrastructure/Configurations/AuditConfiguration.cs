using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configurations;

public class AuditConfiguration : IEntityTypeConfiguration<Audit>
{
public void Configure(EntityTypeBuilder<Audit> builder)
{
       // Table name
       builder.ToTable("audits");

       // Primary key
       builder.HasKey(a => a.Id)
              .HasName("pk_auditoria");

       builder.Property(a => a.Id)
              .HasColumnName("audit_id")
              .ValueGeneratedOnAdd();

       // Properties
       builder.Property(a => a.UserId)
              .HasColumnName("user_id")
              .IsRequired();

       builder.Property(a => a.AffectedEntity)
              .HasColumnName("affected_entity")
              .HasMaxLength(50)
              .IsRequired();

       builder.Property(a => a.ActionId)
              .HasColumnName("action_id")
              .IsRequired();

       builder.Property(a => a.ActionDate)
              .HasColumnName("action_date")
              .HasDefaultValueSql("CURRENT_TIMESTAMP")
              .IsRequired();

       builder.Property(a => a.ActionDescription)
              .HasColumnName("accion_description")
              .HasColumnType("TEXT");

       // Relaciones
       builder.HasOne(a => a.User)
              .WithMany(u => u.Audits)
              .HasForeignKey(a => a.UserId)
              .HasConstraintName("fk_audit_user")
              .OnDelete(DeleteBehavior.Restrict);

       builder.HasOne(a => a.ActionType)
              .WithMany() // TipoAccion no tiene colección de Auditorias (evita error)
              .HasForeignKey(a => a.ActionId)
              .HasConstraintName("fk_audit_type_action")
              .OnDelete(DeleteBehavior.Restrict);

       // Índices
       builder.HasIndex(a => a.UserId)
              .HasDatabaseName("ix_audit_user_id");

       builder.HasIndex(a => a.ActionId)
              .HasDatabaseName("ix_audit_action_id");

       builder.HasIndex(a => a.ActionDate)
              .HasDatabaseName("ix_audit_date_time");
}
}
