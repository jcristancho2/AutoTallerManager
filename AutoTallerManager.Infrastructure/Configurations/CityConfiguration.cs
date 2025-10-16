using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configuration
{
    public class CityConfiguration : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.ToTable("cities");

             // Clave primaria
            builder.HasKey(c => c.Id)
                   .HasName("pk_city");

            builder.Property(c => c.Id)
                   .HasColumnName("id")
                   .ValueGeneratedOnAdd();

            // Propiedades
            builder.Property(c => c.Name)
                   .HasColumnName("name")
                   .HasMaxLength(150)
                   .IsRequired(false); // el modelo permite null

            builder.Property(c => c.DepartmentId)
                   .HasColumnName("state_id")
                   .IsRequired();

            // Relación con Departamento (N:1)
            builder.HasOne(c => c.Department)
                   .WithMany(d => d.Cities)
                   .HasForeignKey(c => c.DepartmentId)
                   .HasConstraintName("fk_department_city")
                   .OnDelete(DeleteBehavior.Restrict);

            // Relación con Direcciones (1:N)
            builder.HasMany(c => c.Addresses)
                   .WithOne(d => d.City)
                   .HasForeignKey(d => d.CityId)
                   .HasConstraintName("fk_address_city")
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}