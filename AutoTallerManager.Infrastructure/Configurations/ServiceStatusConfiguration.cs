using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configuration
{
    public class ServiceStatusConfiguration : IEntityTypeConfiguration<ServiceStatus>
    {
        public void Configure(EntityTypeBuilder<ServiceStatus> builder)
        {
            builder.ToTable("services_status");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                .HasColumnName("status_id");

            builder.Property(e => e.ServiceStatusName)
            .HasColumnName("service_status_name")
                .HasMaxLength(80); 
        }
    }
}