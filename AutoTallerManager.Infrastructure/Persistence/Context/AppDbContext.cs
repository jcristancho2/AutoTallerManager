using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using AutoTallerManager.Domain.Entities;
//sing para la ubicacion de Auth

namespace AutoTallerManager.Infrastructure.Persistence.Context;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    //entidades de autenticacion

    //entidades principales

    //tablas de apoyo

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}
