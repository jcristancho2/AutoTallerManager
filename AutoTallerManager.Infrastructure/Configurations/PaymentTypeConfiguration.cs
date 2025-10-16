using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configuration
{
    public class PaymentTypeConfiguration : IEntityTypeConfiguration<PaymentType>
    {
        public void Configure(EntityTypeBuilder<PaymentType> builder)
        {
            builder.ToTable("payment_types");

            builder.HasKey(tp => tp.Id);
            builder.Property(tp => tp.Id)
                   .HasColumnName("payment_type_id");

            builder.Property(tp => tp.Name)
                   .HasColumnName("name_type_pag")
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasMany(tp => tp.Invoices)
                   .WithOne(f => f.PaymentType)
                   .HasForeignKey(f => f.PaymentTypeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}