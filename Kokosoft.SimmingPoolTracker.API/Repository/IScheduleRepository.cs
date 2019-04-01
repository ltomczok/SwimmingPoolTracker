using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kokosoft.SimmingPoolTracker.API.Data;
using Kokosoft.SimmingPoolTracker.API.Model;

namespace Kokosoft.SimmingPoolTracker.API.Repository
{
    public interface IScheduleRepository
    {
        Task<List<Schedule>> GetSchedule(DateTime date);
    }
}
