using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Infrastructure.Configuration
{
    public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
    {
        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
            builder.ToTable("categories");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                    .HasColumnName("categoriaid");

            builder.Property(c => c.NombreCat)
                    .HasColumnName("nombre_cat")
                   .IsRequired()
                   .HasMaxLength(100);

          
        }
    }
}
