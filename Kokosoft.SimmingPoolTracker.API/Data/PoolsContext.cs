using Kokosoft.SimmingPoolTracker.API.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kokosoft.SimmingPoolTracker.API.Data
{
    public class PoolsContext : DbContext
    {
        public DbSet<Pool> SwimmingPools { get; set; }
        public DbSet<Schedule> Schedules { get; set; }

        public PoolsContext()
        {
        }

        public PoolsContext(DbContextOptions<PoolsContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pool>().HasKey(p => p.ShortName);

            modelBuilder.Entity<Schedule>()
                .Property(p => p.StartTime).HasMaxLength(5);

            modelBuilder.Entity<Schedule>()
                .Property(p => p.EndTime).HasMaxLength(5);

            modelBuilder.Entity<Schedule>()
                .HasAlternateKey(a => new { a.Day, a.StartTime });

            base.OnModelCreating(modelBuilder);
        }
    }
}
