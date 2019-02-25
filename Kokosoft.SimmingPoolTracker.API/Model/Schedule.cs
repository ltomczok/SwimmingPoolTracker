using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kokosoft.SimmingPoolTracker.API.Model
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
    }
}
