using Kokosoft.SimmingPoolTracker.API.Model;
using Kokosoft.SimmingPoolTracker.API.Model.Dto;
using Kokosoft.SimmingPoolTracker.API.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kokosoft.SimmingPoolTracker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PoolsController : ControllerBase
    {
        public PoolsController(IScheduleRepository repo, ILogger<PoolsController> logger)
        {
            this.repo = repo;
            this.logger = logger;
        }

        private IScheduleRepository repo { get; }
        private ILogger<PoolsController> logger { get; }

        // GET api/pools/olimpijczyk/occupancy?date=2019-02-22&time=17:30
        [HttpGet("{pool}/occupancy")]
        public async Task<ActionResult<Occupancy>> GetOccupancy([FromRoute]string pool, [FromQuery]DateTime date, string time)
        {
            try
            {
                Occupancy occupancy = null;
                if (HttpContext.Request.Query.ContainsKey("time"))
                {
                    //for now we omit the {pool} parameter
                     occupancy = await repo.GetOccupancy(date, time);
                }
                else
                {
                    occupancy = await repo.GetOccupancy(date);
                }
                return occupancy;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error during getting occupancy of swimming pool {pool}.", ex);
            }
            return BadRequest();
        }

        // GET api/pools/olimpijczyk/lastoccupancy
        [HttpGet("{pool}/lastoccupancy")]
        public async Task<ActionResult<Occupancy>> GetLastOccupancy([FromRoute]string pool)
        {
            try
            {
                //for now we omit the {pool} parameter
                Occupancy occupancy = await repo.GetLastOccupancy();
                return occupancy;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error during getting last occupancy of swimming pool {pool}.", ex);
            }
            return BadRequest();
        }

        // GET api/pools
        [HttpGet]
        public ActionResult<string> GetPools()
        {
            try
            {
                return "olimpijczyk";
            }
            catch (Exception ex)
            {
                logger.LogError($"Error during getting pools list.", ex);
            }
            return BadRequest();
        }
    }
}
