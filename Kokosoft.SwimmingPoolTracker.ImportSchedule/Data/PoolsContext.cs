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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=localhost;Database=pools;Username=postgres;Password=1234");
    }

}
