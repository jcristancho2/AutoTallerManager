using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("customers");

            // Primary key (GUID)
            builder.HasKey(c => c.Id)
                   .HasName("pk_customer");

            builder.Property(c => c.Id)
                   .HasColumnName("id")
                   .ValueGeneratedOnAdd(); 

            // Basic properties
            builder.Property(c => c.FullName)
                   .HasColumnName("full_name")
                   .HasMaxLength(150)
                   .IsRequired();

            builder.Property(c => c.Phone)
                   .HasColumnName("phone")
                   .HasMaxLength(20)
                   .IsRequired(false);

            builder.Property(c => c.Email)
                   .HasColumnName("email")
                   .HasMaxLength(100)
                   .IsRequired(false);

            // Properties for foreign keys
            builder.Property(c => c.CustomerTypeId)
                   .HasColumnName("customer_type_id")
                   .IsRequired();

            builder.Property(c => c.AddressId)
                   .HasColumnName("address_id")
                   .IsRequired();

            // Relationships
            builder.HasOne<CustomerType>()
                   .WithMany(t => t.Customers)
                   .HasForeignKey(c => c.CustomerTypeId)
                   .HasConstraintName("fk_customer_type_customer")
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Address>()
                   .WithMany(d => d.Customers)
                   .HasForeignKey(c => c.AddressId)
                   .HasConstraintName("fk_address_customer")
                   .OnDelete(DeleteBehavior.Restrict);

            // // Índices opcionales
            // builder.HasIndex(c => c.Email)
            //        .HasDatabaseName("ix_customer_Email");

            // builder.HasIndex(c => c.Phone)
            //        .HasDatabaseName("ix_customer_Phone");
        }
    }
}