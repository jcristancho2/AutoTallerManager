using AutoTallerManager.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configurations.Auth;

public class UserMemberConfiguration : IEntityTypeConfiguration<UserMember>
{
    public void Configure(EntityTypeBuilder<UserMember> builder)
    {
        builder.ToTable("users_members");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Username)
               .HasColumnName("username")
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(u => u.Email)
               .HasColumnName("email")
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(u => u.Password)
               .HasColumnName("password")
               .IsRequired()
               .HasMaxLength(500);

        // Relación N:M con Rol a través de UserMemberRol
        builder.HasMany(u => u.UserMemberRoles)
               .WithOne(umr => umr.UserMember)
               .HasForeignKey(umr => umr.UserMemberId)
               .HasConstraintName("fk_user_member_roles_user_member")
               .OnDelete(DeleteBehavior.Cascade);

        // Relación con RefreshTokens
        builder.HasMany(u => u.RefreshTokens)
               .WithOne(rt => rt.UserMember)
               .HasForeignKey(rt => rt.UserId)
               .HasConstraintName("fk_refresh_tokens_user_member")
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(u => u.Username).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
    }
}
