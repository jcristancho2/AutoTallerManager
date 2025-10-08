using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.API.Configuration
{
    public class TipoPagoConfiguration : IEntityTypeConfiguration<TipoPago>
    {
        public void Configure(EntityTypeBuilder<TipoPago> builder)
        {
            builder.ToTable("payment_types");

            builder.HasKey(tp => tp.PagoId);
            builder.Property(tp => tp.PagoId)
                   .HasColumnName("tipo_pago_id");

            builder.Property(tp => tp.NombreTipoPag)
                   .HasColumnName("nombre_tipo_pag")
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasMany(tp => tp.Facturas)
                   .WithOne(f => f.TipoPago)
                   .HasForeignKey(f => f.PagoId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}