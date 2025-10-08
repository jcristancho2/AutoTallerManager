using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.API.Configuration
{
    public class RepuestoConfiguration : IEntityTypeConfiguration<Repuesto>
    {
        public void Configure(EntityTypeBuilder<Repuesto> builder)
        {
            builder.ToTable("replacement");

            builder.HasKey(r => r.RepuestoId);
                builder.Property(r => r.RepuestoId)
                         .HasColumnName("repuestoid");

                     builder.Property(r => r.Codigo)
                     .HasColumnName("codigo")
                   .IsRequired()
                   .HasMaxLength(50);

                     builder.Property(r => r.NombreRep)
                     .HasColumnName("nombrerep")
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
                     .HasColumnName("preciounitario")
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
