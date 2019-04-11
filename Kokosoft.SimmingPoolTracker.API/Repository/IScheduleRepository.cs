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
        Task<List<Pool>> GetPools();
        Task<Pool> GetPool(string poolId);
        Task<Occupancy> GetOccupancy(DateTime date, string time);
        Task<Occupancy> GetOccupancy(DateTime date);
        Task<Occupancy> GetLastOccupancy();
    }
}
