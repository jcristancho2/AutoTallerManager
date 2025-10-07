using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Infrastructure.Repositories;

    public sealed class AppDbContext (DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        //Aqui van los DbSets
        //public DbSet<YourEntity> YourEntities { get; set; } 
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly); 
    }
