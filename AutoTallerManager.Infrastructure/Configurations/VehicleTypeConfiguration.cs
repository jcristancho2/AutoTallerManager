using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Infrastructure.Configurations
{
    public class VehicleTypeConfiguration : IEntityTypeConfiguration<VehicleType>
    {
        public void Configure(EntityTypeBuilder<VehicleType> builder)
        {
            builder.ToTable("vehicle_types");

            builder.HasKey(tv => tv.Id);
                builder.Property(tv => tv.Id)
                         .HasColumnName("vehicle_type_id");

            builder.Property(tv => tv.VehicleTypeName)
                   .IsRequired()
                   .HasMaxLength(100);

            
        }
    }
}
