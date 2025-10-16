using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configuration
{
    public class DetailOrderConfiguration : IEntityTypeConfiguration<OrderDetail>
       {
              public void Configure(EntityTypeBuilder<OrderDetail> builder)
              {
            builder.ToTable("orders_details");

            builder.HasKey(d => new { d.DetailOrderId, d.ServiceOrderId })
                            .HasName("pk_order_detail");

                     builder.Property(d => d.DetailOrderId)
                            .HasColumnName("detail_order_id")
                            .ValueGeneratedNever();

            builder.Property(d => d.ServiceOrderId)
                   .HasColumnName("service_order_id")
                            .IsRequired();

            builder.Property(d => d.SpareId)
                   .HasColumnName("spare_part_id")
                   .IsRequired(false);

            builder.Property(d => d.Description)
                   .HasColumnName("description")
                   .HasMaxLength(255)
                   .IsRequired(false);

                     builder.Property(d => d.Quantity)
                            .HasColumnName("quantity")
                            .HasDefaultValue(1)
                            .IsRequired();

            builder.Property(d => d.UnitPrice)
                   .HasColumnName("unit_price")
                   .HasPrecision(10, 2)
                   .HasDefaultValue(0m)
                   .IsRequired();

            builder.Property(d => d.LaborCost)
                   .HasColumnName("labor_cost")
                   .HasPrecision(10, 2)
                   .HasDefaultValue(0m)
                   .IsRequired();

                     builder.ToTable(t =>
                     {
                     t.HasCheckConstraint("ck_order_detail_quantity", "quantity > 0");
                     t.HasCheckConstraint("ck_order_detail_pu", "unit_price >= 0");
                     t.HasCheckConstraint("ck_order_detail_labor", "labor_cost >= 0");
                     });

                     // 🔹 Relación con OrdenServicio
            builder.HasOne(d => d.ServiceOrder)
                            .WithMany(o => o.OrderDetails)
                            .HasForeignKey(d => d.ServiceOrderId)
                            .HasConstraintName("fk_order_detail_order_service")
                            .OnDelete(DeleteBehavior.Cascade);

                     // 🔹 Relación con Repuesto (corregida)
            builder.HasOne(d => d.Spare)
                            .WithMany(r => r.OrderDetails)
                   .HasForeignKey(d => d.SpareId)
                   .HasPrincipalKey(r => r.Id)
                   .HasConstraintName("fk_order_detail_spare")
                   .OnDelete(DeleteBehavior.SetNull);

                     builder.HasIndex(d => d.ServiceOrderId)
                            .HasDatabaseName("ix_order_detail_order_service_id");

                     builder.HasIndex(d => d.SpareId)
                            .HasDatabaseName("ix_order_detail_spare_id");
              }
       }

}