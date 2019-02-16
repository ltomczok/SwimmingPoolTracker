using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kokosoft.SwimmingPoolTracker.CheckNewSchedule.Helpers
{
    public class Month
    {
        public string MonthShortName { get; set; }
        public int MonthNumber { get; set; }

        public Month(string shortName, int number)
        {
            MonthShortName = shortName;
            MonthNumber = number;
        }
    }
}
