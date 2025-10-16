using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configuration
{
    public class ServiceTypeConfiguration : IEntityTypeConfiguration<ServiceType>
    {
        public void Configure(EntityTypeBuilder<ServiceType> builder)
        {
            builder.ToTable("service_types");

            builder.HasKey(ts => ts.Id);
            builder.Property(ts => ts.Id)
                   .HasColumnName("service_type");

            builder.Property(ts => ts.ServiceTypeName)
                   .HasColumnName("service_type_name")
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasMany(ts => ts.ServiceOrders)
                   .WithOne(os => os.ServiceType)
                   .HasForeignKey(os => os.ServiceTypeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}