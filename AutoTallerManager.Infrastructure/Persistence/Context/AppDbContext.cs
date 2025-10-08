using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

//using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Infrastructure.Persistence.Context;

public sealed class AppDbContext : DbContext
{
    //public DbSet<Product> Products { get; set; } = default!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
