using System;

namespace Kokosoft.SwimmingPoolTracker.Core
{
    public class NewSchedulle
    {
        public string Id { get; set; }
        public int DayFrom { get; set; }
        public int DayTo { get; set; }
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string Link { get; set; }
        public DateTime ModificationDate { get; set; }
    }
}
