using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configurations;

public class OrdenServicioConfiguration : IEntityTypeConfiguration<OrdenServicio>
{
public void Configure(EntityTypeBuilder<OrdenServicio> builder)
{
       // Nombre de la tabla
       builder.ToTable("ordenes_servicio");

       // Clave primaria
       builder.HasKey(o => o.Id)
              .HasName("pk_orden_servicio");

       builder.Property(o => o.Id)
              .HasColumnName("orden_servicio_id")
              .ValueGeneratedOnAdd();

       // Propiedades principales
       builder.Property(o => o.VehiculoId)
              .HasColumnName("vehiculo_id")
              .IsRequired();

       builder.Property(o => o.MecanicoId)
              .HasColumnName("mecanico_id")
              .IsRequired();

       builder.Property(o => o.FechaIngreso)
              .HasColumnName("fecha_ingreso")
              .HasColumnType("DATE")
              .IsRequired();

       builder.Property(o => o.FechaEstimadaEntrega)
              .HasColumnName("fecha_estimada_entrega")
              .HasColumnType("DATE");

       builder.Property(o => o.TipoServId)
              .HasColumnName("tipo_serv_id")
              .IsRequired();

       builder.Property(o => o.EstadoId)
              .HasColumnName("estado_id")
              .IsRequired();

       // Relaciones
       builder.HasOne(o => o.Vehiculo)
              .WithMany(v => v.OrdenesServicio)
              .HasForeignKey(o => o.VehiculoId)
              .HasConstraintName("fk_orden_servicio_vehiculo")
              .OnDelete(DeleteBehavior.Restrict);

       builder.HasOne(o => o.Mecanico)
              .WithMany(u => u.OrdenesServicio)
              .HasForeignKey(o => o.MecanicoId)
              .HasConstraintName("fk_orden_servicio_mecanico")
              .OnDelete(DeleteBehavior.Restrict);

       builder.HasOne(o => o.TipoServicio)
              .WithMany(t => t.OrdenesServicio)
              .HasForeignKey(o => o.TipoServId)
              .HasConstraintName("fk_orden_servicio_tipo_servicio")
              .OnDelete(DeleteBehavior.Restrict);

       builder.HasOne(o => o.Estado)
              .WithMany(e => e.OrdenesServicio)
              .HasForeignKey(o => o.EstadoId)
              .HasConstraintName("fk_orden_servicio_estado")
              .OnDelete(DeleteBehavior.Restrict);

       // Relación DetalleOrden (uno a muchos)
       builder.HasMany(o => o.DetallesOrden)
              .WithOne(d => d.OrdenServicio)
              .HasForeignKey(d => d.OrdenServicioId)
              .HasConstraintName("fk_orden_servicio_detalle")
              .OnDelete(DeleteBehavior.Cascade);

       // Relación Factura (uno a muchos)
       builder.HasMany(o => o.Facturas)
              .WithOne(f => f.OrdenServicio)
              .HasForeignKey(f => f.OrdenServicioId)
              .HasConstraintName("fk_orden_servicio_factura")
              .OnDelete(DeleteBehavior.Restrict);

       // Índices útiles
       builder.HasIndex(o => o.EstadoId)
              .HasDatabaseName("ix_orden_servicio_estado_id");

       builder.HasIndex(o => o.FechaIngreso)
              .HasDatabaseName("ix_orden_servicio_fecha_ingreso");
}
}
