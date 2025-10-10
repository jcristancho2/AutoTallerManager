using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configuration
{
    public class EstadoServConfiguration : IEntityTypeConfiguration<EstadoServ>
    {
        public void Configure(EntityTypeBuilder<EstadoServ> builder)
        {
            builder.ToTable("services_status");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                .HasColumnName("estado_id");

            builder.Property(e => e.NombreEstServ)
            .HasColumnName("nombre_est_serv")
                .HasMaxLength(80); 
        }
    }
}