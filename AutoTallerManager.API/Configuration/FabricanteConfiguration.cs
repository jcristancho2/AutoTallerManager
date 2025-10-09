using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.API.Configuration
{
     public class FabricanteConfiguration : IEntityTypeConfiguration<Fabricante>
    {
        public void Configure(EntityTypeBuilder<Fabricante> builder)
        {
            builder.ToTable("manufacturer");

            builder.HasKey(f => f.FabricanteId);
            builder.Property(f => f.FabricanteId)
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
                     
            builder.Property(f => f.Correo)
                     .HasColumnName("correo")
                     .HasMaxLength(80);
        }
    }
}