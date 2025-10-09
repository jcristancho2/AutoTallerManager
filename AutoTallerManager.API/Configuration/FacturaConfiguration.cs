using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.API.Configuration
{
     public class FacturaConfiguration : IEntityTypeConfiguration<Factura>
    {
        public void Configure(EntityTypeBuilder<Factura> builder)
        {
            builder.ToTable("bills");

            // 🔑 Clave primaria
            builder.HasKey(f => f.FacturaId);

            builder.Property(f => f.FacturaId)
                   .HasColumnName("factura_id")
                   .ValueGeneratedOnAdd();

            // 🧾 Relación con OrdenServicio (1:1 o 1:N según modelo)
            builder.Property(f => f.OrdenServicioId)
                   .HasColumnName("orden_servicio_id")
                   .IsRequired();

      builder.HasOne(f => f.OrdenServicio)
             .WithOne(o => o.Factura)
             .HasForeignKey<Factura>(f => f.OrdenServicioId)
             .OnDelete(DeleteBehavior.Restrict);
                

            // 👤 Relación con Cliente
            builder.Property(f => f.ClienteId)
                   .HasColumnName("cliente_id")
                   .IsRequired();

            builder.HasOne(f => f.Cliente)
                   .WithMany(c => c.Facturas)
                   .HasForeignKey(f => f.ClienteId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .OnUpdate(DeleteBehavior.Cascade);

            // 💳 Relación con TipoPago
            builder.Property(f => f.PagoId)
                   .HasColumnName("pago_id")
                   .IsRequired();

                builder.HasOne(f => f.TipoPago)
                   .WithMany(p => p.Facturas)
                   .HasForeignKey(f => f.PagoId)
                   .OnDelete(DeleteBehavior.Restrict);

            // 📅 Fecha
            builder.Property(f => f.Fecha)
                   .HasColumnName("fecha")
                   .HasColumnType("date")
                   .IsRequired();

            // 💰 Total con restricción >= 0
            builder.Property(f => f.Total)
                   .HasColumnName("total")
                   .HasColumnType("decimal(10,2)")
                   .IsRequired();

            // ✅ Restricción CHECK
            builder.HasCheckConstraint("CK_Factura_Total_Positive", "total >= 0");

            // 🔒 Único en OrdenServicioId
            builder.HasIndex(f => f.OrdenServicioId)
                   .IsUnique();
        }
    }
}
    
