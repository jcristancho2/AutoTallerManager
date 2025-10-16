using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configuration
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
             builder.ToTable("addresses");

            builder.HasKey(d => d.Id)
                   .HasName("pk_address");

            builder.Property(d => d.Id)
                   .HasColumnName("id")
                   .ValueGeneratedOnAdd();

            builder.Property(d => d.Description)
                   .HasColumnName("description")
                   .HasMaxLength(150)
                   .IsRequired();

            builder.Property(d => d.CityId)
                   .HasColumnName("city_id")
                   .IsRequired();

            builder.HasOne(d => d.City)
                   .WithMany(c => c.Addresses) // asumiendo que Ciudad tiene ICollection<Direccion>
                   .HasForeignKey(d => d.CityId)
                   .HasConstraintName("fk_address_city")
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}