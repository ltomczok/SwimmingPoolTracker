using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kokosoft.SwimmingPoolTracker.CheckNewSchedule.Helpers
{
    public interface IMonthHelpers
    {
        bool CheckIfMonth(string monthName);
        int GetMonthNumber(string monthName);
    }

    public class MonthHelpers : IMonthHelpers
    {
        private List<Month> monthNames = new List<Month>();
        public MonthHelpers()
        {
            monthNames.Add(new Month("sty", 1));
            monthNames.Add(new Month("lut", 2));
            monthNames.Add(new Month("mar", 3));
            monthNames.Add(new Month("kwi", 4));
            monthNames.Add(new Month("maj", 5));
            monthNames.Add(new Month("cze", 6));
            monthNames.Add(new Month("lip", 7));
            monthNames.Add(new Month("sie", 8));
            monthNames.Add(new Month("wrz", 9));
            monthNames.Add(new Month("paź", 10));
            monthNames.Add(new Month("lis", 11));
            monthNames.Add(new Month("gru", 12));
        }

        public bool CheckIfMonth(string monthName)
        {
            if (monthName.Length < 3)
            {
                return false;
            }
            string monthShortName = monthName.ToLower().Substring(0, 3);
            bool exist = monthNames.Where(m => m.MonthShortName == monthShortName).Any();
            return exist;
        }

        public int GetMonthNumber(string monthName)
        {
            string monthShortName = monthName.ToLower().Substring(0, 3);
            Month month = monthNames.Where(m => m.MonthShortName == monthShortName).SingleOrDefault();
            return month.MonthNumber;
        }
    }
}
