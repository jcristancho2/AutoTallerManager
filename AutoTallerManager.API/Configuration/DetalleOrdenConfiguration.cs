using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.API.Configuration
{
    public class DetalleOrdenConfiguration : IEntityTypeConfiguration<DetalleOrden>
    {
      public void Configure(EntityTypeBuilder<DetalleOrden> builder)
        {
            builder.ToTable("orders_details");

                     // PK compuesta (DetalleOrdenId, OrdenServicioId)
                     builder.HasKey(d => new { d.DetalleOrdenId, d.OrdenServicioId })
                           .HasName("pk_order_detail"); //tengo dudas aquui en esta linea

            builder.Property(d => d.DetalleOrdenId)
                         .HasColumnName("detalleordenid");

              builder.Property(d => d.OrdenServicioId)
                         .HasColumnName("ordenservicioid");

                     // Si usas MySQL y el DDL tiene AUTO_INCREMENT,
                     // muchos proveedores/ORM no soportan identity en PK compuesta.
                     // Puedes intentar:
              builder.Property(d => d.DetalleOrdenId)
                     .HasColumnName("detalleordenid");

              builder.Property(d => d.RepuestoId)
                     .HasColumnName("repuestoid")
                     .IsRequired();

              builder.Property(d => d.Descripcion)
                   .HasColumnName("descripcion")
                   .IsRequired()
                   .HasMaxLength(255);

              builder.Property(d => d.Cantidad)
                   .HasColumnName("cantidad")
                   .IsRequired()
                   .HasDefaultValue(1);

              builder.Property(d => d.PrecioUnitario)
                   .HasColumnName("preciounitario")
                   .IsRequired()
                   .HasColumnType("decimal(10,2)")
                   .HasDefaultValue(0m);

              builder.Property(d => d.PrecioManoDeObra)
                   .HasColumnName("preciomanodeobra")
                   .IsRequired()
                   .HasColumnType("decimal(10,2)")
                   .HasDefaultValue(0m);

            // CHECK constraints
            builder.HasCheckConstraint("ck_order_detail_cantidad", "cantidad > 0");
            builder.HasCheckConstraint("ck_order_detail_pu", "preciounitario >= 0");
            builder.HasCheckConstraint("ck_order_detail_mano", "preciomanodeobra >= 0");
            // FK con OrdenServicio (ON DELETE CASCADE)
            builder.HasOne(d => d.OrdenServicio)
                   .WithMany(o => o.Detalles)          // Asegúrate de tener ICollection<DetalleOrden>? Detalles en OrdenServicio
                   .HasForeignKey(d => d.OrdenServicioId)
                   .OnDelete(DeleteBehavior.Cascade);

            // FK con Repuesto (ON DELETE RESTRICT)
            builder.HasOne(d => d.Repuesto)
                   .WithMany()                         
                   .HasForeignKey(d => d.RepuestoId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}