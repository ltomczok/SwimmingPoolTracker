using System;

namespace Kokosoft.SwimmingPoolTracker.Core
{
    public class NewSchedule
    {
        public Pools Pool { get; set; }
        public string Id { get; set; }
        public int DayFrom { get; set; }
        public int DayTo { get; set; }
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string Link { get; set; }
        public DateTime ModificationDate { get; set; }

        public DateTime StartDate
        {
            get
            {
                return new DateTime(this.YearFrom, this.MonthFrom, this.DayFrom);
            }
        }

        public DateTime EndDate
        {
            get
            {
                return new DateTime(this.YearTo, this.MonthTo, this.DayTo);
            }
        }
    }
}
