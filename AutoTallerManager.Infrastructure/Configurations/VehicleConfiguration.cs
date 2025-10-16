using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configurations
{
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.ToTable("vehicles");

            builder.HasKey(v => v.Id);
            
            builder.Property(v => v.Id)
                   .HasColumnName("id")
                   .ValueGeneratedOnAdd();

            builder.Property(v => v.VIN)
                   .HasColumnName("vin")
                   .HasMaxLength(17);

            builder.Property(v => v.Year)
                   .HasColumnName("year")
                   .IsRequired();

            builder.Property(v => v.Mileage)
                   .HasColumnName("mileage")
                   .IsRequired();

            builder.Property(v => v.Plate)
                   .HasColumnName("plate")
                   .HasMaxLength(20);

            builder.Property(v => v.CustomerId)
                   .HasColumnName("customer_id")
                   .IsRequired();

            builder.Property(v => v.VehicleTypeId)
                   .HasColumnName("vehice_type_id")
                   .IsRequired();

            builder.Property(v => v.VehicleBrandId)
                   .HasColumnName("vehicle_brand_id")
                   .IsRequired();

            builder.Property(v => v.VehicleModelId)
                   .HasColumnName("vehicle_model_id")
                   .IsRequired();

            builder.Property(v => v.CreatedAt)
                   .HasColumnName("created_at")
                   .IsRequired();

            builder.Property(v => v.UpdatedAt)
                   .HasColumnName("updated_at")
                   .IsRequired();

            // Índices únicos
            builder.HasIndex(v => v.VIN)
                   .IsUnique();

            builder.HasIndex(v => v.Plate)
                   .IsUnique();

            // Check constraints usando la nueva sintaxis
            builder.ToTable(t => t.HasCheckConstraint("CK_Vehicle_Mileage", "Mileage >= 0"));
            builder.ToTable(t => t.HasCheckConstraint("CK_Vehicle_Year", "Year >= 1900 AND Year <= 2030"));

            // Relaciones
            builder.HasOne(v => v.Customer)
                   .WithMany(c => c.Vehicles)
                   .HasForeignKey(v => v.CustomerId)
                   .HasConstraintName("FK_Customer_Vehicles_CustomerId")
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.VehicleType)
                   .WithMany(tv => tv.Vehicles)
                   .HasForeignKey(v => v.VehicleTypeId)
                   .HasConstraintName("FK_Vehicles_Vehicletypes_VehicleTypeId")
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.VehicleBrand)
                   .WithMany(mv => mv.Vehicles)
                   .HasForeignKey(v => v.VehicleBrandId)
                   .HasConstraintName("FK_Vehicles_VehicleBrand_VehicleBrandId")
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.VehicleModel)
                   .WithMany(mv => mv.Vehicles)
                   .HasForeignKey(v => v.VehicleModelId)
                   .HasConstraintName("FK_Vehicles_VehicleModels_VehicleIdModel")
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}