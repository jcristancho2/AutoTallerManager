using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configuration
{
     public class FabricanteConfiguration : IEntityTypeConfiguration<Fabricante>
    {
        public void Configure(EntityTypeBuilder<Fabricante> builder)
        {
            builder.ToTable("manufacturer");

            builder.HasKey(f => f.Id);
            builder.Property(f => f.Id)
                   .HasColumnName("fabricanteid");

            builder.Property(f => f.NombreFab)
                     .HasColumnName("nombrefab")
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(f => f.Descripcion)
                     .HasColumnName("descripcion")
                     .HasMaxLength(255);

            builder.Property(f => f.Telefono)
                     .HasColumnName("telefono")
                     .HasMaxLength(20);
                     
            builder.Property(f => f.Email)
                     .HasColumnName("email")
                     .HasMaxLength(80);
        }
    }
}