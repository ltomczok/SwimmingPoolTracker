using Kokosoft.SimmingPoolTracker.API.Model;
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

        // GET api/pools/5/schedule?date=2019-02-22&time=17:30
        [HttpGet("{id}/schedule")]
        public async Task<ActionResult<Schedule>> GetSchedule([FromRoute]int id, [FromQuery]DateTime date, string time)
        {
            try
            {
                //so far we omit the {id} parameter
                Schedule schedule = await repo.GetSchedule(date, time);
                return schedule;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error during getting schedule of swimming pool {id}.", ex);
            }
            return BadRequest();
        }
    }
}
