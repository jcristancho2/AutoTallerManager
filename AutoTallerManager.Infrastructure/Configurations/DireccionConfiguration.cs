using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configuration
{
    public class DireccionConfiguration : IEntityTypeConfiguration<Direccion>
    {
        public void Configure(EntityTypeBuilder<Direccion> builder)
        {
             builder.ToTable("addresses");

            builder.HasKey(d => d.Id)
                   .HasName("pk_direccion");

            builder.Property(d => d.Id)
                   .HasColumnName("id")
                   .ValueGeneratedOnAdd();

            builder.Property(d => d.Descripcion)
                   .HasColumnName("descripcion")
                   .HasMaxLength(250) // longitud razonable para direcciones
                   .IsRequired(false); // permite nulo, ya que es string?

            builder.Property(d => d.CiudadId)
                   .HasColumnName("ciudad_id")
                   .IsRequired();

            builder.HasOne(d => d.Ciudad)
                   .WithMany(c => c.Direcciones) // asumiendo que Ciudad tiene ICollection<Direccion>
                   .HasForeignKey(d => d.CiudadId)
                   .HasConstraintName("fk_direccion_ciudad")
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}