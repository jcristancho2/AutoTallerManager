using AutoTallerManager.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AutoTallerManager.Infrastructure.Configurations.Auth
{
    public class RolConfiguration : IEntityTypeConfiguration<Rol>
    {
        public void Configure(EntityTypeBuilder<Rol> builder)
    {
        builder.ToTable("rols");

            builder.HasKey(r => r.Id);
            builder.Property(di => di.Id)
                .ValueGeneratedOnAdd()
                .IsRequired()
                .HasColumnName("id");

            builder.Property(p => p.Name)
            .HasColumnName("rolName")
            .HasColumnType("varchar")
            .HasMaxLength(50)
            .IsRequired();

            builder.Property(r => r.Description)
                .IsRequired()
                .HasMaxLength(50);

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
        }
    }
}
