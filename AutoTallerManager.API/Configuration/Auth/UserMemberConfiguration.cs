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
        builder.HasMany(u => u.UserMemberRoles)
               .WithOne(umr => umr.UserMember)
               .HasForeignKey(umr => umr.UserMemberId)
               .OnDelete(DeleteBehavior.Cascade);

                builder.HasMany(p => p.RefreshTokens)
                .WithOne(p => p.UserMember)
                .HasForeignKey(p => p.UserId);
    }
}