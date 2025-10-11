using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configuration
{
    public class DetalleOrdenConfiguration : IEntityTypeConfiguration<DetalleOrden>
       {
              public void Configure(EntityTypeBuilder<DetalleOrden> builder)
              {
                     builder.ToTable("orders_details");

                     builder.HasKey(d => new { d.DetalleOrdenId, d.OrdenServicioId })
                            .HasName("pk_order_detail");

                     builder.Property(d => d.DetalleOrdenId)
                            .HasColumnName("detalle_orden_id")
                            .ValueGeneratedNever();

                     builder.Property(d => d.OrdenServicioId)
                            .HasColumnName("orden_servicio_id")
                            .IsRequired();

                     builder.Property(d => d.RepuestoId)
                            .HasColumnName("repuesto_id")
                            .IsRequired(false);

                     builder.Property(d => d.Descripcion)
                            .HasColumnName("descripcion")
                            .HasMaxLength(255)
                            .IsRequired(false);

                     builder.Property(d => d.Cantidad)
                            .HasColumnName("cantidad")
                            .HasDefaultValue(1)
                            .IsRequired();

                     builder.Property(d => d.PrecioUnitario)
                            .HasColumnName("precio_unitario")
                            .HasPrecision(10, 2)
                            .HasDefaultValue(0m)
                            .IsRequired();

                     builder.Property(d => d.PrecioManoDeObra)
                            .HasColumnName("precio_mano_de_obra")
                            .HasPrecision(10, 2)
                            .HasDefaultValue(0m)
                            .IsRequired();

                     builder.ToTable(t =>
                     {
                     t.HasCheckConstraint("ck_order_detail_cantidad", "cantidad > 0");
                     t.HasCheckConstraint("ck_order_detail_pu", "precio_unitario >= 0");
                     t.HasCheckConstraint("ck_order_detail_mano", "precio_mano_de_obra >= 0");
                     });

                     // 🔹 Relación con OrdenServicio
                     builder.HasOne(d => d.OrdenServicio)
                            .WithMany(o => o.DetallesOrden)
                            .HasForeignKey(d => d.OrdenServicioId)
                            .HasConstraintName("fk_order_detail_orden_servicio")
                            .OnDelete(DeleteBehavior.Cascade);

                     // 🔹 Relación con Repuesto (corregida)
                     builder.HasOne(d => d.Repuesto)
                            .WithMany(r => r.DetallesOrden)
                            .HasForeignKey(d => d.RepuestoId)
                            .HasPrincipalKey(r => r.Id) // 👈 EF ahora sabe que la PK de Repuesto es Id
                            .HasConstraintName("fk_order_detail_repuesto")
                            .OnDelete(DeleteBehavior.SetNull);

                     builder.HasIndex(d => d.OrdenServicioId)
                            .HasDatabaseName("ix_order_detail_orden_servicio_id");

                     builder.HasIndex(d => d.RepuestoId)
                            .HasDatabaseName("ix_order_detail_repuesto_id");
              }
       }

}