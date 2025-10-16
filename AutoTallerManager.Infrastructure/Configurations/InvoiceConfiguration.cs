using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configuration
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.ToTable("bills");

            // Clave primaria
            builder.HasKey(f => f.Id);

            builder.Property(f => f.Id)
                   .HasColumnName("invoice_id")
                   .ValueGeneratedOnAdd();

            // Relación con OrdenServicio (1:N)
            builder.Property(f => f.ServiceOrderId)
                   .HasColumnName("order_service_id")
                   .IsRequired();

            builder.HasOne(f => f.ServiceOrder)
                   .WithMany(o => o.Invoices) 
                   .HasForeignKey(f => f.ServiceOrderId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Relación con Cliente (1:N)
            builder.Property(f => f.CustomerId)
                   .HasColumnName("customer_id")
                   .IsRequired();

            builder.HasOne(f => f.Customer)
                   .WithMany(c => c.Invoices)
                   .HasForeignKey(f => f.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Relación con TipoPago (1:N)
            builder.Property(f => f.PaymentTypeId)
                   .HasColumnName("payment_id")
                   .IsRequired();

            builder.HasOne(f => f.PaymentType)
                   .WithMany(p => p.Invoices)
                   .HasForeignKey(f => f.PaymentTypeId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Fecha
            builder.Property(f => f.InvoiceDate)
                   .HasColumnName("date")
                   .HasColumnType("date")
                   .IsRequired();

            // Total con restricción >= 0
            builder.Property(f => f.Total)
                   .HasColumnName("total")
                   .HasColumnType("decimal(10,2)")
                   .IsRequired();

            // Check constraint: total positivo
            builder.ToTable(t => t.HasCheckConstraint("CK_Invoice_Total_Positive", "total >= 0"));
        }
    }
}
