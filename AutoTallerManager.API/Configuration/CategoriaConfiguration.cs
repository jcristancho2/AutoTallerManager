using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.API.Configuration
{
    public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
    {
        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
            builder.ToTable("categories");

            builder.HasKey(c => c.CategoriaId);
            builder.Property(c => c.CategoriaId)
                    .HasColumnName("categoriaid");

            builder.Property(c => c.NombreCat)
                    .HasColumnName("nombre_cat")
                   .IsRequired()
                   .HasMaxLength(100);

          
        }
    }
}
