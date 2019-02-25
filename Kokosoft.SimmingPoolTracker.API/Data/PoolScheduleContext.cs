using Kokosoft.SimmingPoolTracker.API.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kokosoft.SimmingPoolTracker.API.Data
{
    public class PoolScheduleContext : DbContext
    {
        public DbSet<Pool> SwimmingPools { get; set; }
        public DbSet<Schedule> Schedules { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=localhost;Database=pools;Username=postgres;Password=1234");
    }
}
