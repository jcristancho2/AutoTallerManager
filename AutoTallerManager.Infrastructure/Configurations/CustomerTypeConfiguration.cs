using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configuration
{
    public class CustomerTypeConfiguration : IEntityTypeConfiguration<CustomerType>
    {
        public void Configure(EntityTypeBuilder<CustomerType> builder)
        {
           builder.ToTable("customers_types");

            // Clave primaria
            builder.HasKey(t => t.Id)
                   .HasName("pk_customer_type");

            builder.Property(t => t.Id)
                   .HasColumnName("id")
                   .ValueGeneratedOnAdd();

            // Propiedades
            builder.Property(t => t.Name)
                   .HasColumnName("name")
                   .HasMaxLength(100)
                   .IsRequired();

            // Relación 1:N con Cliente
            builder.HasMany(t => t.Customers)
                   .WithOne()
                   .HasForeignKey(c => c.CustomerTypeId)
                   .HasConstraintName("fk_customer_type_customer")
                   .OnDelete(DeleteBehavior.Restrict);

            // Índice opcional para búsquedas o validaciones
            builder.HasIndex(t => t.Name)
                   .HasDatabaseName("ix_customer_type_name");
        }
    }
}