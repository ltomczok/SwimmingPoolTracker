using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kokosoft.SwimmingPoolTracker.CheckNewSchedule.Model
{
    public class PoolSchedule
    {
        public string Id
        {
            get
            {
                return this.ToString();
            }
        }

        public override string ToString()
        {
            return $"{FillDate(DayFrom)}{FillDate(MonthFrom)}{YearFrom}{FillDate(DayTo)}{FillDate(MonthTo)}{YearTo}";
        }

        internal string FillDate(int date)
        {
            string dateString = date.ToString();
            if (dateString.Length < 2)
            {
                dateString = $"0{dateString}";
            }
            return dateString;
        }

        public int DayFrom { get; set; }
        public int MonthFrom { get; set; }
        public int YearFrom { get; set; }
        public DateTime DateFrom
        {
            get
            {
                return new DateTime(YearFrom, MonthFrom, DayFrom);
            }
        }

        public int DayTo { get; set; }
        public int MonthTo { get; set; }
        public int YearTo { get; set; }

        public DateTime DateTo
        {
            get
            {
                return new DateTime(YearTo, MonthTo, DayTo);
            }
        }

        public DateTime ModificationDate { get; set; }
        public string Link { get; set; }
        internal void CheckDates()
        {
            if (MonthFrom == 0)
            {
                MonthFrom = MonthTo;
            }
            if (YearFrom == 0)
            {
                YearFrom = YearTo;
            }
        }
    }
}
