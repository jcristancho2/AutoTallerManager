using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Infrastructure.Configuration
{
    public class SpareConfiguration : IEntityTypeConfiguration<Spare>
    {
        public void Configure(EntityTypeBuilder<Spare> builder)
        {
            builder.ToTable("spare_parts");

            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id)
                   .HasColumnName("id");

                     builder.Property(r => r.Code)
                     .HasColumnName("code")
                   .IsRequired()
                   .HasMaxLength(50);

                     builder.Property(r => r.Name)
                     .HasColumnName("name")
                   .IsRequired()
                   .HasMaxLength(150);

                     builder.Property(r => r.Description)
                     .HasColumnName("description")
                   .IsRequired()
                   .HasMaxLength(255);

                     builder.Property(r => r.Stock)
                     .HasColumnName("stock")
                   .IsRequired()
                   .HasDefaultValue(0);

                     builder.Property(r => r.UnitPrice)
                     .HasColumnName("unit_price")
                   .HasPrecision(10, 2)
                   .IsRequired();
            builder.Property(r => r.StockMin)
                   .HasColumnName("stock_min")
                   .IsRequired();

            builder.Property(r => r.CategoryId)
                   .HasColumnName("category_id")
                   .IsRequired();

            builder.Property(r => r.VehicleTypeId)
                   .HasColumnName("vehicle_type_id")
                   .IsRequired();

            builder.Property(r => r.ManufacturerId)
                   .HasColumnName("manufacturer_id")
                   .IsRequired();

            // Relaciones
            builder.HasOne(r => r.Category)
                   .WithMany(c => c.Spares)
                   .HasForeignKey(r => r.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.VehicleType)
                   .WithMany(tv => tv.Spares)
                   .HasForeignKey(r => r.VehicleTypeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Manufacturer)
                   .WithMany(f => f.Spares)
                   .HasForeignKey(r => r.ManufacturerId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
