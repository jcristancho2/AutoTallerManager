using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configurations;

public class OrderServiceConfiguration : IEntityTypeConfiguration<ServiceOrder>
{
public void Configure(EntityTypeBuilder<ServiceOrder> builder)
{
       // Nombre de la tabla
       builder.ToTable("order_services");

       // Clave primaria
       builder.HasKey(o => o.Id)
              .HasName("pk_order_service");

       builder.Property(o => o.Id)
              .HasColumnName("order_service_id")
              .ValueGeneratedOnAdd();

       // Propiedades principales
       builder.Property(o => o.VehicleId)
              .HasColumnName("vehicle_id")
              .IsRequired();

       builder.Property(o => o.MechanicId)
              .HasColumnName("mechanic_id")
              .IsRequired();

       builder.Property(o => o.EntryDate)
              .HasColumnName("entry_date")
              .HasColumnType("DATE")
              .IsRequired();

       builder.Property(o => o.EstimatedDeliveryDate)
              .HasColumnName("estimated_delivery_date")
              .HasColumnType("DATE");

       builder.Property(o => o.WorkDescription)
              .HasColumnName("work_description")
              .IsRequired();

       builder.Property(o => o.ServiceTypeId)
              .HasColumnName("service_type_id")
              .IsRequired();

       // Relaciones
       builder.HasOne(o => o.Vehicle)
              .WithMany(v => v.ServiceOrders)
              .HasForeignKey(o => o.VehicleId)
              .HasConstraintName("fk_order_service_vehicle")
              .OnDelete(DeleteBehavior.Restrict);

       builder.HasOne(o => o.Mechanic)
              .WithMany(u => u.ServiceOrders)
              .HasForeignKey(o => o.MechanicId)
              .HasConstraintName("fk_order_service_mechanic")
              .OnDelete(DeleteBehavior.Restrict);

       builder.HasOne(o => o.ServiceType)
              .WithMany(t => t.ServiceOrders)
              .HasForeignKey(o => o.ServiceTypeId)
              .HasConstraintName("fk_order_service_type")
              .OnDelete(DeleteBehavior.Restrict);

       builder.HasOne(o => o.ServiceStatus)
              .WithMany(e => e.ServiceOrders)
              .HasForeignKey(o => o.StatusId)
              .HasConstraintName("fk_order_service_status")
              .OnDelete(DeleteBehavior.Restrict);

       // Relación DetalleOrden (uno a muchos)
       builder.HasMany(o => o.OrderDetails)
              .WithOne(d => d.ServiceOrder)
              .HasForeignKey(d => d.ServiceOrderId)
              .HasConstraintName("fk_order_service_order_detail")
              .OnDelete(DeleteBehavior.Cascade);

       // Relación Factura (uno a muchos)
       builder.HasMany(o => o.Invoices)
              .WithOne(f => f.ServiceOrder)
              .HasForeignKey(f => f.ServiceOrderId)
              .HasConstraintName("fk_order_service_invoice")
              .OnDelete(DeleteBehavior.Restrict);

       // Índices útiles
       builder.HasIndex(o => o.StatusId)
              .HasDatabaseName("ix_order_service_status_id");

       builder.HasIndex(o => o.EntryDate)
              .HasDatabaseName("ix_order_service_entry_date");
}
}
