using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.API.Configuration.Auth;

    public class UserMemberConfiguration : IEntityTypeConfiguration<UserMember>
    {
         public void Configure(EntityTypeBuilder<UserMember> builder)
    {
        builder.ToTable("users_members");

        builder.HasKey(u => u.Id);
        builder.Property(di => di.Id)
               .ValueGeneratedOnAdd()
               .IsRequired()
               .HasColumnName("id");

        builder.Property(u => u.Username)
            .HasMaxLength(50)
            .IsRequired()
            .HasColumnName("name");

        builder.Property(u => u.Email)
            .HasMaxLength(50)
            .IsRequired()
            .HasColumnName("email");

        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.Password)
           .HasColumnType("varchar")
           .HasMaxLength(255)
           .IsRequired();

        builder.Property(e => e.CreatedAt)
            .HasColumnName("createdAt")
            .HasColumnType("date")
            .HasDefaultValueSql("CURRENT_DATE")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.UpdatedAt)
            .HasColumnName("updatedAt")
            .HasColumnType("date")
            .HasDefaultValueSql("CURRENT_DATE")
            .ValueGeneratedOnAddOrUpdate();
        builder
               .HasMany(p => p.Rols)
               .WithMany(r => r.UsersMembers)
               .UsingEntity<UserMemberRol>(

                   j => j
                   .HasOne(pt => pt.Rol)
                   .WithMany(t => t.UserMemberRols)
                   .HasForeignKey(ut => ut.RolId),

                   j => j
                   .HasOne(et => et.UserMembers)
                   .WithMany(et => et.UserMemberRols)
                   .HasForeignKey(el => el.UserMemberId),

                   j =>
                   {
                       j.ToTable("users_rols");
                       j.HasKey(t => new { t.UserMemberId, t.RolId });

                   });

                builder.HasMany(p => p.RefreshTokens)
                .WithOne(p => p.UserMember)
                .HasForeignKey(p => p.UserId);
    }
}