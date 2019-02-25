using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kokosoft.SimmingPoolTracker.API.Data;
using Kokosoft.SimmingPoolTracker.API.Model;

namespace Kokosoft.SimmingPoolTracker.API.Repository
{
    public interface IScheduleRepository
    {
        Task<Schedule> GetSchedule(DateTime date, string time);
    }
}
