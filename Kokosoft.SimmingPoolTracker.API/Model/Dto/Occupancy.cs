using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kokosoft.SimmingPoolTracker.API.Model.Dto
{
    public class Occupancy
    {
        public DateTime Date { get; set; }
        public List<Schedule> Schedules { get; set; }
        
        public Occupancy(DateTime day)
        {
            this.Date = day;
            this.Schedules = new List<Schedule>();
        }

        public void AddSchedule(string startTime, string endTime, List<string> tracks)
        {
            this.Schedules.Add(new Schedule()
            {
                StartTime=startTime,
                EndTime=endTime,
                Tracks=new List<string>(tracks)
            });
        }
    }

    public class Schedule
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public List<string> Tracks { get; set; }
    }
}
