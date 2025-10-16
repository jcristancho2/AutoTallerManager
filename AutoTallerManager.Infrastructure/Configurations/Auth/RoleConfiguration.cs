using System;
using AutoTallerManager.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configurations.Auth;

public class RolConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("rols");

        // Primary Key
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .IsRequired();

        // Properties
        builder.Property(r => r.RoleName)
            .HasColumnName("role_name")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.Description)
            .HasColumnName("description")
            .HasMaxLength(200);

        builder.Property(r => r.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        // 
        builder
            .HasMany(r => r.UserMemberRols)  
            .WithOne(umr => umr.Role)
            .HasForeignKey(umr => umr.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
    