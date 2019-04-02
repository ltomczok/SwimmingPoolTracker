using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kokosoft.SimmingPoolTracker.API.Data;
using Kokosoft.SimmingPoolTracker.API.Model;
using Kokosoft.SimmingPoolTracker.API.Model.Dto;

namespace Kokosoft.SimmingPoolTracker.API.Repository
{
    public interface IScheduleRepository
    {
        Task<Occupancy> GetOccupancy(DateTime date, string time);
        Task<Occupancy> GetLastOccupancy();
    }
}
