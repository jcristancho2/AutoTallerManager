using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Infrastructure.Configuration
{
    public class RepuestoConfiguration : IEntityTypeConfiguration<Repuesto>
    {
        public void Configure(EntityTypeBuilder<Repuesto> builder)
        {
            builder.ToTable("replacement");

            builder.HasKey(r => r.Id);
                builder.Property(r => r.Id)
                         .HasColumnName("repuesto_id");

                     builder.Property(r => r.Codigo)
                     .HasColumnName("codigo")
                   .IsRequired()
                   .HasMaxLength(50);

                     builder.Property(r => r.NombreRepu)
                     .HasColumnName("nombre_rep")
                   .IsRequired()
                   .HasMaxLength(150);

                     builder.Property(r => r.Descripcion)
                     .HasColumnName("descripcion")
                   .IsRequired()
                   .HasMaxLength(100);

                     builder.Property(r => r.Stock)
                     .HasColumnName("stock")
                   .IsRequired()
                   .HasDefaultValue(0);

                     builder.Property(r => r.PrecioUnitario)
                     .HasColumnName("precio_unitario")
                   .HasColumnType("decimal(10,2)")
                   .IsRequired();

            // Relaciones
            builder.HasOne(r => r.Categoria)
                   .WithMany(c => c.Repuestos)
                   .HasForeignKey(r => r.CategoriaId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.TipoVehiculo)
                   .WithMany(tv => tv.Repuestos)
                   .HasForeignKey(r => r.TipoVehiculoId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Fabricante)
                   .WithMany(f => f.Repuestos)
                   .HasForeignKey(r => r.FabricanteId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
