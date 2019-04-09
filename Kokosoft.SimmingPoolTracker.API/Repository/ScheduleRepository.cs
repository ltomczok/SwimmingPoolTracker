using Kokosoft.SimmingPoolTracker.API.Data;
using Kokosoft.SimmingPoolTracker.API.Model;
using Kokosoft.SimmingPoolTracker.API.Model.Dto;
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

        public async Task<Occupancy> GetOccupancy(DateTime date, string time)
        {
            Occupancy occupancy = new Occupancy(date);

            List<Model.Schedule> schedules = await dc.Schedules.Where(s => s.Day == date).ToListAsync();
            if (schedules.Count > 0)
            {
                TimeSpan startTime = TimeSpan.Parse(time);

                schedules.Where(s => (TimeSpan.Parse(s.StartTime) >= startTime))
                    .OrderBy(s => s.StartTime)
                    .ToList()
                    .ForEach(s => occupancy.AddSchedule(s.StartTime, s.EndTime, s.Tracks));
            }
            return occupancy;
        }

        public async Task<Occupancy> GetLastOccupancy()
        {
            Model.Schedule lastSchedule = await dc.Schedules.OrderByDescending(s => s.Day).FirstOrDefaultAsync();
            Occupancy lastOccupancy = new Occupancy(lastSchedule.Day);
            return lastOccupancy;
        }        
    }
}
