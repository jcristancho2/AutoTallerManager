using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configuration
{
    public class TipoPagoConfiguration : IEntityTypeConfiguration<TipoPago>
    {
        public void Configure(EntityTypeBuilder<TipoPago> builder)
        {
            builder.ToTable("payment_types");

            builder.HasKey(tp => tp.Id);
            builder.Property(tp => tp.Id)
                   .HasColumnName("tipo_pago_id");

            builder.Property(tp => tp.NombreTipoPag)
                   .HasColumnName("nombre_tipo_pag")
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasMany(tp => tp.Facturas)
                   .WithOne(f => f.TipoPago)
                   .HasForeignKey(f => f.TipoPagoId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}