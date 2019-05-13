using Kokosoft.SwimmingPoolTracker.CheckNewSchedule;
using Kokosoft.SwimmingPoolTracker.CheckNewSchedule.Helpers;
using Kokosoft.SwimmingPoolTracker.CheckNewSchedule.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CheckNewScheduleTests
{
    public class GetValuesFromDateStringShould
    {
        [Theory]
        [InlineData("6", DateType.From)]
        public void SetCorrectValues(string date, DateType dateType)
        {
            PoolSchedule poolSchedule = new PoolSchedule();
            CheckNewScheduleBackgroundService check = new CheckNewScheduleBackgroundService(new MonthHelpers(), null, null, null, null);
            check.GetValuesFromDateString(date, poolSchedule, dateType);

            Assert.Equal(6, poolSchedule.DayFrom);
        }
    }
}
