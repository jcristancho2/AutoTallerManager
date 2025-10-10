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

            // PK compuesta
            builder.HasKey(d => new { d.DetalleOrdenId, d.OrdenServicioId })
                   .HasName("pk_order_detail");

            // Columnas
            builder.Property(d => d.DetalleOrdenId)
                   .HasColumnName("detalle_orden_id")
                   .ValueGeneratedNever(); // NO identity en PK compuesta

            builder.Property(d => d.OrdenServicioId)
                   .HasColumnName("orden_servicio_id")
                   .IsRequired();

            builder.Property(d => d.RepuestoId)
                   .HasColumnName("repuesto_id")
                   .IsRequired(false); // nullable

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

            // Configurar tabla con check constraints
            builder.ToTable(t => {
                t.HasCheckConstraint("ck_order_detail_cantidad", "cantidad > 0");
                t.HasCheckConstraint("ck_order_detail_pu", "precio_unitario >= 0");
                t.HasCheckConstraint("ck_order_detail_mano", "precio_mano_de_obra >= 0");
            });

            // Relaciones
            builder.HasOne(d => d.OrdenServicio)
                   .WithMany(o => o.DetallesOrden)
                   .HasForeignKey(d => d.OrdenServicioId)
                   .HasConstraintName("fk_order_detail_orden_servicio")
                   .OnDelete(DeleteBehavior.Cascade);

            // Repuesto es opcional: si eliminas el repuesto, puedes dejar SetNull para mantener historial
            builder.HasOne(d => d.Repuesto)
                   .WithMany()
                   .HasForeignKey(d => d.RepuestoId)
                   .HasConstraintName("fk_order_detail_repuesto")
                   .OnDelete(DeleteBehavior.SetNull);

            // Índices útiles
            builder.HasIndex(d => d.OrdenServicioId).HasDatabaseName("ix_order_detail_orden_servicio_id");
            builder.HasIndex(d => d.RepuestoId).HasDatabaseName("ix_order_detail_repuesto_id");
        }
    }
}