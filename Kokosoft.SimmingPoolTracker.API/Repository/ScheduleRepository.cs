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

        public async Task<List<Model.Dto.Pool>> GetPools()
        {
            List<Model.Dto.Pool> databasePools = await dc.SwimmingPools.Include(p => p.Address).Select(p => new Model.Dto.Pool()
            {
                ShortName = p.ShortName,
                Name = p.Name,
                OpenTime = p.OpenTime,
                CloseTime = p.CloseTime,
                ExitTime = p.ExitTime,
                TracksCount = p.TracksCount,
                Length = p.Length,
                Width = p.Width,
                Street = p.Address.Street,
                City = p.Address.City,
                ZipCode = p.Address.ZipCode
            }).ToListAsync();

            return databasePools;
        }

        public async Task<Model.Dto.Pool> GetPool(string poolId)
        {
            Model.Dto.Pool pool = await dc.SwimmingPools
                .Include(p => p.Address)
                .Where(p => p.ShortName == poolId)
                .Select(p => new Model.Dto.Pool()
                {
                    ShortName = p.ShortName,
                    Name = p.Name,
                    OpenTime = p.OpenTime,
                    CloseTime = p.CloseTime,
                    ExitTime = p.ExitTime,
                    TracksCount = p.TracksCount,
                    Length = p.Length,
                    Width = p.Width,
                    Street = p.Address.Street,
                    City = p.Address.City,
                    ZipCode = p.Address.ZipCode
                })
                .SingleOrDefaultAsync();
            return pool;
        }

        public async Task<Occupancy> GetOccupancy(DateTime date)
        {
            Occupancy occupancy = new Occupancy(date);

            List<Model.Schedule> schedules = await dc.Schedules.Where(s => s.Day == date).ToListAsync();
            if (schedules.Count > 0)
            {
                schedules.OrderBy(s => s.StartTime)
                    .ToList()
                    .ForEach(s => occupancy.AddSchedule(s.StartTime, s.EndTime, s.Tracks));
            }
            if (occupancy.Schedules.Count > 0)
            {
                return occupancy;
            }
            return null;
        }

        public async Task<Occupancy> GetOccupancy(DateTime date, string time)
        {
            Occupancy occupancy = new Occupancy(date);

            List<Model.Schedule> schedules = await dc.Schedules.Where(s => s.Day == date).ToListAsync();
            if (schedules.Count > 0)
            {
                TimeSpan getTime = TimeSpan.Parse(time);

                schedules.Where(s => (getTime <= TimeSpan.Parse(s.EndTime)))
                    .OrderBy(s => s.StartTime)
                    .ToList()
                    .ForEach(s => occupancy.AddSchedule(s.StartTime, s.EndTime, s.Tracks));
            }
            if (occupancy.Schedules.Count > 0)
            {
                return occupancy;
            }
            return null;
        }

        public async Task<Occupancy> GetLastOccupancy()
        {
            Model.Schedule lastSchedule = await dc.Schedules.OrderByDescending(s => s.Day).FirstOrDefaultAsync();
            Occupancy lastOccupancy = new Occupancy(lastSchedule.Day);
            return lastOccupancy;
        }
    }
}
