using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Infrastructure.Configurations
{
    public class TipoVehiculoConfiguration : IEntityTypeConfiguration<TipoVehiculo>
    {
        public void Configure(EntityTypeBuilder<TipoVehiculo> builder)
        {
            builder.ToTable("vehicle_types");

            builder.HasKey(tv => tv.Id);
                builder.Property(tv => tv.Id)
                         .HasColumnName("tipo_vehiculo_id");

            builder.Property(tv => tv.NombreTipoVehiculo)
                   .IsRequired()
                   .HasMaxLength(100);

            
        }
    }
}
