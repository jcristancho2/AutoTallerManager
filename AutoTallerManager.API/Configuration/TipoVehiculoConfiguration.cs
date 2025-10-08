using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.API.Configuration
{
    public class TipoVehiculoConfiguration : IEntityTypeConfiguration<TipoVehiculo>
    {
        public void Configure(EntityTypeBuilder<TipoVehiculo> builder)
        {
            builder.ToTable("vehicle_types");

            builder.HasKey(tv => tv.TipoVehiculoId);
                builder.Property(tv => tv.TipoVehiculoId)
                         .HasColumnName("tipo_vehiculo_id");

            builder.Property(tv => tv.NombreTipoVehi)
                   .IsRequired()
                   .HasMaxLength(100);

            
        }
    }
}
