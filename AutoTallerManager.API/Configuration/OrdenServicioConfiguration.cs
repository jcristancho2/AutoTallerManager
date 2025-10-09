using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Infrastructure.Configurations
{
    public class OrdenServicioConfiguration : IEntityTypeConfiguration<OrdenServicio>
    {
              public void Configure(EntityTypeBuilder<OrdenServicio> builder)
              {
                     // Nombre de tabla (alineado a tu DDL)
                     builder.ToTable("service_orders");

                     // PK
                     builder.HasKey(o => o.OrdenServicioId);

                     builder.Property(o => o.OrdenServicioId)
                            .HasColumnName("orden_servicio_id")
                            .ValueGeneratedOnAdd();

                     // Campos simples
                     builder.Property(o => o.FechaIngreso)
                            .HasColumnName("fecha_ingreso")
                            .HasColumnType("date")
                            .IsRequired();

                     builder.Property(o => o.FechaEstimadaEntrega)
                            .HasColumnName("fecha_estimada_entrega")
                            .HasColumnType("date")
                            .IsRequired();

                     // FKs (asegúrate de que existen estas props en la entidad)
                     builder.Property(o => o.VehiculoId)
                            .HasColumnName("vehiculo_id")
                            .IsRequired();

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
                            .OnDelete(DeleteBehavior.Restrict);

                     builder.HasOne(o => o.TipoServicio)
                            .WithMany(ts => ts.OrdenesServicio)
                            .HasForeignKey(o => o.TipoServId)
                            .OnDelete(DeleteBehavior.Restrict);

                     builder.HasOne(o => o.Estado)
                            .WithMany(e => e.OrdenesServicio)
                            .HasForeignKey(o => o.EstadoId)
                            .OnDelete(DeleteBehavior.Restrict);

                     // Configurar la tabla con el check constraint
                     builder.ToTable(t => t.HasCheckConstraint(
                        "ck_service_orders_fechas",
                        "fecha_estimada_entrega >= fecha_ingreso"
                     )); 
            
            // Índices recomendados (rendimiento de consultas/joins)
            builder.HasIndex(o => o.VehiculoId)
                   .HasDatabaseName("ix_service_orders_vehiculo_id");

            builder.HasIndex(o => o.TipoServId)
                   .HasDatabaseName("ix_service_orders_tipo_serv_id");

            builder.HasIndex(o => o.EstadoId)
                   .HasDatabaseName("ix_service_orders_estado_id");
        }
    }
}
