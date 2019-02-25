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
        public string Time { get; set; }
        public int Track50 { get; set; }
        public int Track25 { get; set; }
        public int TrackShallow { get; set; }
        public Pool Pool { get; set; }

        public bool IsEmpty
        {
            get
            {
                return this.Track25.Equals(0) && this.Track50.Equals(0) && this.TrackShallow.Equals(0);
            }
        }
    }
}
