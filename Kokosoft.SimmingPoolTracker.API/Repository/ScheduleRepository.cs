using Kokosoft.SimmingPoolTracker.API.Data;
using Kokosoft.SimmingPoolTracker.API.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kokosoft.SimmingPoolTracker.API.Repository
{
    public class ScheduleRepository : IScheduleRepository
    {
        private PoolsContext dc { get; }
        public ScheduleRepository(PoolsContext dc)
        {
            this.dc = dc;
        }

        public async Task<List<Schedule>> GetSchedule(DateTime date)
        {           
            List<Schedule> schedule = await dc.Schedules.Where(s => s.Day == date).ToListAsync();
            return schedule;
        }    
    }
}
