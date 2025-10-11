using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configurations.Auth
{
    public class UserMemberRolConfiguration : IEntityTypeConfiguration<UserMemberRol>
    {
       public void Configure(EntityTypeBuilder<UserMemberRol> builder)
        {
            builder.ToTable("user_member_roles");

            // Clave compuesta (N:M entre UserMember y Rol)
            builder.HasKey(umr => new { umr.UserMemberId, umr.RolId })
                   .HasName("pk_user_member_rol");

            // Propiedades
            builder.Property(umr => umr.UserMemberId)
                   .HasColumnName("user_member_id")
                   .IsRequired();

            builder.Property(umr => umr.RolId)
                   .HasColumnName("rol_id")
                   .IsRequired();

            // Relaciones
            builder.HasOne(umr => umr.UserMember)
                   .WithMany(um => um.UserMemberRoles) 
                   .HasForeignKey(umr => umr.UserMemberId)
                   .HasConstraintName("fk_user_member_rol_user_member")
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(umr => umr.Rol)
                   .WithMany(r => r.UserMemberRoles) 
                   .HasForeignKey(umr => umr.RolId)
                   .HasConstraintName("fk_user_member_rol_rol")
                   .OnDelete(DeleteBehavior.Restrict);

            // Índices
            builder.HasIndex(umr => umr.RolId)
                   .HasDatabaseName("ix_user_member_rol_rol_id");

            builder.HasIndex(umr => umr.UserMemberId)
                   .HasDatabaseName("ix_user_member_rol_user_member_id");
        } 
    }
}