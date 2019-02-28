using Kokosoft.SwimmingPoolTracker.ImportSchedule.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kokosoft.SwimmingPoolTracker.ImportSchedule.Data
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
            modelBuilder.Entity<Schedule>()
                .Property(p => p.Time).HasMaxLength(5);

            modelBuilder.Entity<Schedule>()
                .HasAlternateKey(a => new { a.Day, a.Time });

            base.OnModelCreating(modelBuilder);
        }
    }

}
