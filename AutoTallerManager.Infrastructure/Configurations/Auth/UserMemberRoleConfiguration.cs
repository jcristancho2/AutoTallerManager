using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configurations.Auth
{
    public class UserMemberRoleConfiguration : IEntityTypeConfiguration<UserMemberRole>
    {
       public void Configure(EntityTypeBuilder<UserMemberRole> builder)
        {
            builder.ToTable("user_member_rols");

            // Clave compuesta (N:M entre UserMember y Rol)
            builder.HasKey(umr => new { umr.UserMemberId, umr.RoleId })
                   .HasName("pk_user_member_rols");

            // Propiedades
            builder.Property(umr => umr.UserMemberId)
                   .HasColumnName("user_member_id")
                   .IsRequired();

            builder.Property(umr => umr.RoleId)
                   .HasColumnName("role_id")
                   .IsRequired();

            // Relaciones
            builder.HasOne(umr => umr.UserMember)
                   .WithMany(um => um.UserMemberRols) 
                   .HasForeignKey(umr => umr.UserMemberId)
                   .HasConstraintName("fk_user_member_role_user_member")
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(umr => umr.Role)
                   .WithMany(r => r.UserMemberRols) 
                   .HasForeignKey(umr => umr.RoleId)
                   .HasConstraintName("fk_user_member_rol_rol")
                   .OnDelete(DeleteBehavior.Restrict);

            // Índices
            builder.HasIndex(umr => umr.RoleId)
                   .HasDatabaseName("ix_user_member_rol_rol_id");

            builder.HasIndex(umr => umr.UserMemberId)
                   .HasDatabaseName("ix_user_member_rol_user_member_id");
        } 
    }
}