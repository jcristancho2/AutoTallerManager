using System;
using AutoTallerManager.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configurations.Auth;

public class RolConfiguration : IEntityTypeConfiguration<Rol>
{
    public void Configure(EntityTypeBuilder<Rol> builder)
    {
        builder.ToTable("rols");

        // Primary Key
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .IsRequired();

        // Properties
        builder.Property(r => r.NombreRol)
            .HasColumnName("nombre_rol")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.Descripcion)
            .HasColumnName("descripcion")
            .HasMaxLength(200);

        builder.Property(r => r.Activo)
            .HasColumnName("activo")
            .HasDefaultValue(true);

        // ✅ CORREGIR: UserMemberRols existe en entidad
        builder
            .HasMany(r => r.UserMemberRoles)  // ✅ CORREGIR: usar UserMemberRols
            .WithOne(umr => umr.Rol)
            .HasForeignKey(umr => umr.RolId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
    