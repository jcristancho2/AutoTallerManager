using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configuration
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.ToTable("countries");

            // Clave primaria
            builder.HasKey(p => p.Id)
                   .HasName("pk_country");

            builder.Property(p => p.Id)
                   .HasColumnName("id")
                   .ValueGeneratedOnAdd();

            // Propiedades
            builder.Property(p => p.Name)
                   .HasColumnName("name")
                   .HasMaxLength(150)
                   .IsRequired();

            // Relación 1:N con Departamentos
            builder.HasMany(p => p.Departments)
                   .WithOne(d => d.Country)
                   .HasForeignKey(d => d.CountryId)
                   .HasConstraintName("fk_department_country")
                   .OnDelete(DeleteBehavior.Restrict);

            // Índice opcional por nombre
            builder.HasIndex(p => p.Name)
                   .HasDatabaseName("ix_country_name");
        }
    }
}