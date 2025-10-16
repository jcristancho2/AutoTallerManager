using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configuration
{
     public class ManufacturerConfiguration : IEntityTypeConfiguration<Manufacturer>
    {
        public void Configure(EntityTypeBuilder<Manufacturer> builder)
        {
            builder.ToTable("manufacturer");

            builder.HasKey(f => f.Id);
            builder.Property(f => f.Id)
                   .HasColumnName("Manufacturerid");

            builder.Property(f => f.Name)
                     .HasColumnName("nombrefab")
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(f => f.Description)
                     .HasColumnName("description")
                     .HasMaxLength(255);

            builder.Property(f => f.Phone)
                     .HasColumnName("phone")
                     .HasMaxLength(20);
                     
            builder.Property(f => f.Email)
                     .HasColumnName("email")
                     .HasMaxLength(80);
        }
    }
}