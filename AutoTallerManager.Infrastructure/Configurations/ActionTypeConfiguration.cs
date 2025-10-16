using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Configuration
{
    public class ActionTypeConfiguration : IEntityTypeConfiguration<ActionType>
    {
        public void Configure(EntityTypeBuilder<ActionType> builder)
        {
            builder.ToTable("action_types");

            // Clave primaria
            builder.HasKey(t => t.Id)
                   .HasName("pk_action_type");

            builder.Property(t => t.Id)
                   .HasColumnName("id")
                   .ValueGeneratedOnAdd();

            // Propiedades
            builder.Property(t => t.ActionName)
                   .HasColumnName("name_action")
                   .HasMaxLength(100)
                   .IsRequired();

            // Índice opcional para búsquedas
            builder.HasIndex(t => t.ActionName)
                   .HasDatabaseName("ix_action_ty_action_name");
        }
    }
 }