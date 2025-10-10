using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configuration
{
    public class TipoServicioConfiguration : IEntityTypeConfiguration<TipoServicio>
    {
        public void Configure(EntityTypeBuilder<TipoServicio> builder)
        {
            builder.ToTable("service_types");

            builder.HasKey(ts => ts.Id);
            builder.Property(ts => ts.Id)
                   .HasColumnName("tipo_servid");

            builder.Property(ts => ts.NombreTipoServ)
                   .HasColumnName("nombre_tipo_serv")
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasMany(ts => ts.OrdenesServicio)
                   .WithOne(os => os.TipoServicio)
                   .HasForeignKey(os => os.TipoServId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}