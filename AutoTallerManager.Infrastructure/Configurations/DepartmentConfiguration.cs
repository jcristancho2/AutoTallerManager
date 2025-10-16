using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configuration
{
    public class DepartamentoConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("departments");

             // PK
            builder.HasKey(d => d.Id)
                   .HasName("pk_department");

            builder.Property(d => d.Id)
                   .HasColumnName("id")
                   .ValueGeneratedOnAdd();

            // Propiedades
            builder.Property(d => d.Name)
                   .HasColumnName("name")
                   .HasMaxLength(150)
                   .IsRequired(false); // permite null

            builder.Property(d => d.CountryId)
                   .HasColumnName("country_id")
                   .IsRequired();

            // Relación con Pais (N:1)
            builder.HasOne(d => d.Country)
                   .WithMany(p => p.Departments) 
                   .HasForeignKey(d => d.CountryId)
                   .HasConstraintName("fk_department_country")
                   .OnDelete(DeleteBehavior.Restrict);

            // Relación con Ciudad (1:N)
            builder.HasMany(d => d.Cities)
                   .WithOne(c => c.DepartmentId)
                   .HasForeignKey(c => c.DepartmentId) 
                   .HasConstraintName("fk_city_department")
                   .OnDelete(DeleteBehavior.Restrict);

            // Índices útiles
            builder.HasIndex(d => d.CountryId)
                   .HasDatabaseName("ix_departamento_pais_id");

            builder.HasIndex(d => d.Name)
                   .HasDatabaseName("ix_departamento_nombre");
        }
    }
}