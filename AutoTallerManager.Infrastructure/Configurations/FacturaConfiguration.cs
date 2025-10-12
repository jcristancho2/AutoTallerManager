using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configuration
{
    public class FacturaConfiguration : IEntityTypeConfiguration<Factura>
    {
        public void Configure(EntityTypeBuilder<Factura> builder)
        {
            builder.ToTable("bills");

            // 🔑 Clave primaria
            builder.HasKey(f => f.Id);

            builder.Property(f => f.Id)
                   .HasColumnName("factura_id")
                   .ValueGeneratedOnAdd();

            // 🧾 Relación con OrdenServicio (1:N)
            builder.Property(f => f.OrdenServicioId)
                   .HasColumnName("orden_servicio_id")
                   .IsRequired();

            builder.HasOne(f => f.OrdenServicio)
                   .WithMany(o => o.Facturas) // ✅ relación correcta
                   .HasForeignKey(f => f.OrdenServicioId)
                   .OnDelete(DeleteBehavior.Restrict);

            // 👤 Relación con Cliente (1:N)
            builder.Property(f => f.ClienteId)
                   .HasColumnName("cliente_id")
                   .IsRequired();

            builder.HasOne(f => f.Cliente)
                   .WithMany(c => c.Facturas)
                   .HasForeignKey(f => f.ClienteId)
                   .OnDelete(DeleteBehavior.Restrict);

            // 💳 Relación con TipoPago (1:N)
            builder.Property(f => f.TipoPagoId)
                   .HasColumnName("pago_id")
                   .IsRequired();

            builder.HasOne(f => f.TipoPago)
                   .WithMany(p => p.Facturas)
                   .HasForeignKey(f => f.TipoPagoId)
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

            // 🧩 Check constraint: total positivo
            builder.ToTable(t => t.HasCheckConstraint("CK_Factura_Total_Positive", "total >= 0"));
        }
    }
}
