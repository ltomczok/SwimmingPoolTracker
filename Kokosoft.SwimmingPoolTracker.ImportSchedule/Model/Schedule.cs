using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kokosoft.SwimmingPoolTracker.ImportSchedule.Model
{
    public class Schedule
    {
        public int Id { get; set; }
        public DateTime Day { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public List<string> Tracks { get; set; }
        public Pool Pool { get; set; }

        public Schedule()
        {
            Tracks = new List<string>();
        }

        public bool IsEmpty
        {
            get
            {
                return this.Tracks.Count() == 0;
            }
        }
    }
}
